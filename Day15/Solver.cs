namespace Day15
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using CommonCode.Machine;
    using CommonCode.Machine.DefaultOps;

    internal class Solver : ISolver
    {
        public enum Direction
        {
            North = 1,
            South = 2,
            West = 3,
            East = 4
        }

        public enum DroidState
        {
            Explore,
            CalculateDistance
        }

        public enum TileType
        {
            Open,
            OxygenSystem,
            Wall,
            Unknown
        }

        public int Solve(Data inputData)
        {
            var cts = new CancellationTokenSource();
            var data = inputData.OpCodes.Split(',').Select(long.Parse).ToArray();
            var intMachine = new IntMachine(
                (1, new Add()),
                (2, new Multiply()),
                (3, new Input()),
                (4, new Output()),
                (5, new JmpTrue()),
                (6, new JmpFalse()),
                (7, new LessThan()),
                (8, new Equals()),
                (9, new OffsetRelativeBase()),
                (99, new Break())
            )
            {
                EnableExtendedOpCodeSupport = true,
                MinimumBufferSize = 32 * 1024
            };

            var machineToServerChannel = Channel.CreateUnbounded<long>();
            var serverToMachineChannel = Channel.CreateUnbounded<long>();

            intMachine.InputRequested += (sender, args) =>
            {
                args.ValueAsync = serverToMachineChannel.Reader.ReadAsync(cts.Token);
            };
            intMachine.Output += (sender, args) =>
            {
                machineToServerChannel.Writer.WriteAsync(args.Output, cts.Token);
            };
            intMachine.Completed += (sender, args) =>
            {
                machineToServerChannel.Writer.Complete();
                serverToMachineChannel.Writer.Complete();
            };

            var tiles = new Dictionary<(long X, long Y), ExploredTile>();
            (long X, long Y) position = (0, 0);

            this.AddTile(position, tiles, TileType.Open);
            var state = DroidState.Explore;
            var result = Task.Run(async () =>
            {
                Queue<Direction> path = null;
                while (!cts.IsCancellationRequested)
                {
                    if (state == DroidState.Explore && (path == null || path.Count <= 0))
                    {
                        var steps = this.DecideMovement(position, tiles);
                        if (steps != null && steps.Length > 0)
                        {
                            path = new Queue<Direction>(steps);
                        }
                        else
                        {
                            state = DroidState.CalculateDistance;
                        }
                    }

                    switch (state)
                    {
                        case DroidState.Explore:
                            Debug.Assert(path != null, nameof(path) + " != null");
                            var nextDirection = (long)path.Dequeue();
                            await serverToMachineChannel.Writer.WriteAsync(nextDirection, cts.Token);
                            var result = await machineToServerChannel.Reader.ReadAsync(cts.Token);
                            switch (result)
                            {
                                case 0:
                                    this.AddTile(MovePosition(position, (Direction)nextDirection), tiles,
                                        TileType.Wall);
                                    path.Clear();
                                    break;
                                case 1:
                                    position = MovePosition(position, (Direction)nextDirection);
                                    this.AddTile(position, tiles, TileType.Open);
                                    break;
                                case 2:
                                    position = MovePosition(position, (Direction)nextDirection);
                                    this.AddTile(position, tiles, TileType.OxygenSystem);
                                    break;
                            }

                            break;
                        case DroidState.CalculateDistance:
                            var pathToOxygen = DijkstraSearch((0, 0), t => t.Type == TileType.OxygenSystem, tiles);
                            cts.Cancel();
                            return pathToOxygen.Length;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return 0;
            }, cts.Token);

            Task.Run(async () =>
            {
                try
                {
                    await intMachine.ProcessAsync(data, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // Ignored
                }
            }, cts.Token);
            return result.Result;
        }

        private static Direction[] DijkstraSearch(
            (long X, long Y) start,
            Func<ExploredTile, bool> searchCondition,
            IReadOnlyDictionary<(long X, long Y), ExploredTile> tiles)
        {
            HashSet<(long, long)> encounteredCells = new HashSet<(long, long)>();

            var pathQueues = new Queue<(Direction[] Path, (long X, long Y) EndPoint)>();
            foreach (var direction in tiles[start].OpenNeighbours)
            {
                pathQueues.Enqueue((new[] { direction }, MovePosition(start, direction)));
            }

            while (pathQueues.Count > 0)
            {
                var (path, endPoint) = pathQueues.Dequeue();
                if (!encounteredCells.Add(endPoint))
                {
                    continue;
                }

                var cell = tiles[endPoint];
                if (searchCondition(cell))
                {
                    return path;
                }

                foreach (var direction in cell.OpenNeighbours)
                {
                    pathQueues.Enqueue((path.Concat(new[] { direction }).ToArray(), MovePosition(endPoint, direction)));
                }
            }

            return null;
        }

        private static IEnumerable<((long X, long Y) Position, Direction direction)> GetAdjacentPositions(
            (long X, long Y) p)
        {
            yield return (MovePosition(p, Direction.North), Direction.North);
            yield return (MovePosition(p, Direction.South), Direction.South);
            yield return (MovePosition(p, Direction.West), Direction.West);
            yield return (MovePosition(p, Direction.East), Direction.East);
        }

        private static (long X, long Y) MovePosition((long X, long Y) p, Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return (p.X, p.Y - 1);
                case Direction.South:
                    return (p.X, p.Y + 1);
                case Direction.West:
                    return (p.X - 1, p.Y);
                case Direction.East:
                    return (p.X + 1, p.Y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private void AddTile((long X, long Y) tilePosition, Dictionary<(long X, long Y), ExploredTile> tiles,
            TileType tile)
        {
            if (tiles.ContainsKey(tilePosition))
            {
                return;
            }

            tiles[tilePosition] = ExploredTile.Create(tilePosition, tile, tiles);
            foreach (var (position, direction) in GetAdjacentPositions(tilePosition))
            {
                if (tiles.TryGetValue(position, out var neighbourCell))
                {
                    neighbourCell.SetNeighbour(this.Reverse(direction), tile);
                }
            }
        }

        private Direction[] DecideMovement((long X, long Y) position, Dictionary<(long X, long Y), ExploredTile> tiles)
        {
            var current = tiles[position];
            if (current.HasUnexploredNeighbours)
            {
                var direction = current.UnexploredNeighbours.First();
                var queue = new[]
                {
                    direction,
                    this.Reverse(direction)
                };

                return queue;
            }

            return this.JourneyToNearestEmpty(position, tiles);
        }

        private Direction[] JourneyToNearestEmpty((long X, long Y) position,
            Dictionary<(long X, long Y), ExploredTile> tiles)
        {
            return DijkstraSearch(position, (t) => t.HasUnexploredNeighbours, tiles);
        }

        private Direction Reverse(Direction direction)
        {
            switch (direction)
            {
                case Direction.North: return Direction.South;
                case Direction.South: return Direction.North;
                case Direction.West: return Direction.East;
                case Direction.East: return Direction.West;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public class ExploredTile
        {
            public bool HasUnexploredNeighbours => this.UnexploredNeighbours.Any();

            public TileType[] Neighbours { get; private set; }

            public IEnumerable<Direction> OpenNeighbours
            {
                get
                {
                    if (this.Neighbours[(int)Direction.North - 1] == TileType.Open ||
                        (this.Neighbours[(int)Direction.North - 1] == TileType.OxygenSystem))
                    {
                        yield return Direction.North;
                    }

                    if (this.Neighbours[(int)Direction.South - 1] == TileType.Open ||
                        (this.Neighbours[(int)Direction.South - 1] == TileType.OxygenSystem))
                    {
                        yield return Direction.South;
                    }

                    if (this.Neighbours[(int)Direction.West - 1] == TileType.Open ||
                        (this.Neighbours[(int)Direction.West - 1] == TileType.OxygenSystem))
                    {
                        yield return Direction.West;
                    }

                    if (this.Neighbours[(int)Direction.East - 1] == TileType.Open ||
                        (this.Neighbours[(int)Direction.East - 1] == TileType.OxygenSystem))
                    {
                        yield return Direction.East;
                    }
                }
            }

            public (long X, long Y) Position { get; private set; }
            public TileType Type { get; private set; }

            public IEnumerable<Direction> UnexploredNeighbours
            {
                get
                {
                    if (this.Neighbours[(int)Direction.North - 1] == TileType.Unknown)
                    {
                        yield return Direction.North;
                    }

                    if (this.Neighbours[(int)Direction.South - 1] == TileType.Unknown)
                    {
                        yield return Direction.South;
                    }

                    if (this.Neighbours[(int)Direction.West - 1] == TileType.Unknown)
                    {
                        yield return Direction.West;
                    }

                    if (this.Neighbours[(int)Direction.East - 1] == TileType.Unknown)
                    {
                        yield return Direction.East;
                    }
                }
            }

            public static ExploredTile Create((long X, long Y) p, TileType type,
                Dictionary<(long X, long Y), ExploredTile> exploredTiles)
            {
                var northPos = MovePosition(p, Direction.North);
                var southPos = MovePosition(p, Direction.South);
                var westPos = MovePosition(p, Direction.West);
                var eastPos = MovePosition(p, Direction.East);

                var tile = new ExploredTile
                {
                    Position = p,
                    Type = type,
                    Neighbours = new[]
                    {
                        exploredTiles.ContainsKey(northPos)
                            ? exploredTiles[northPos].Type
                            : TileType.Unknown,
                        exploredTiles.ContainsKey(southPos)
                            ? exploredTiles[southPos].Type
                            : TileType.Unknown,
                        exploredTiles.ContainsKey(westPos)
                            ? exploredTiles[westPos].Type
                            : TileType.Unknown,
                        exploredTiles.ContainsKey(eastPos)
                            ? exploredTiles[eastPos].Type
                            : TileType.Unknown,
                    }
                };

                return tile;
            }

            public void SetNeighbour(Direction direction, TileType tile)
            {
                this.Neighbours[(int)(direction) - 1] = tile;
            }
        }

        /*private static char[,] GetMap(Dictionary<(long X, long Y), Tile> cells)
        {
            var minX = long.MaxValue;
            var maxX = long.MinValue;
            var minY = long.MaxValue;
            var maxY = long.MinValue;
            foreach (var p in cells)
            {
                minX = Math.Min(p.Key.X, minX);
                maxX = Math.Max(p.Key.X, maxX);
                minY = Math.Min(p.Key.Y, minY);
                maxY = Math.Max(p.Key.Y, maxY);
            }

            var width = maxX - minX;
            var height = maxY - minY;

            var output = new char[width + 1, height + 1];
            foreach (var position in cells)
            {
                switch (position.Value)
                {
                    case Tile.Empty:
                        output[position.Key.X - minX, position.Key.Y] = ' ';
                        break;
                    case Tile.Wall:
                        output[position.Key.X - minX, position.Key.Y] = '@';
                        break;
                    case Tile.Block:
                        output[position.Key.X - minX, position.Key.Y] = 'x';
                        break;
                    case Tile.HorizontalPaddle:
                        output[position.Key.X - minX, position.Key.Y] = '_';
                        break;
                    case Tile.Ball:
                        output[position.Key.X - minX, position.Key.Y] = 'o';
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return output;
        }

        private void PrintMap(char[,] map, in long score)
        {
            var sb = new StringBuilder();
            for (var y = 0; y < map.GetLength(1); y++)
            {
                for (var x = 0; x < map.GetLength(0); x++)
                {
                    sb.Append(map[x, y]);
                }

                sb.Append('\n');
            }

            Debug.WriteLine($"\n\nMap:\n{sb}\nScore: {score}");
        }*/
    }
}
