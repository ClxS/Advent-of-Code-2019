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
    using Extensions;
    using Utility;

    internal class Solver : ISolver
    {
        public int Solve(Data inputData)
        {
            var cts = new CancellationTokenSource();
            var data = inputData.OpCodes.Split(',').Select(long.Parse).ToArray();
            var intMachine = IntMachine.CreateDefault();

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

            AddTile(position, tiles, TileType.Open);
            var state = inputData.States.Dequeue();
            var result = Task.Run(async () =>
            {
                Queue<Direction> path = null;
                while (!cts.IsCancellationRequested)
                {
                    if (state == DroidState.Explore && (path == null || path.Count <= 0))
                    {
                        var steps = DecideMovement(position, tiles);
                        if (steps != null && steps.Length > 0)
                        {
                            path = new Queue<Direction>(steps);
                        }
                        else
                        {
                            // Exploration done, move to next state
                            state = inputData.States.Dequeue();
                        }
                    }

                    switch (state)
                    {
                        case DroidState.Explore:
                            position = await this.Explore(position, path, serverToMachineChannel, machineToServerChannel, tiles, cts.Token);
                            break;
                        case DroidState.CalculateDistance:
                            var pathToOxygen = PathingUtility.DijkstraSearch((0, 0), t => t.Type == TileType.OxygenSystem, tiles);
                            cts.Cancel();
                            return pathToOxygen.Length;
                        case DroidState.CalculateOxygenDistributionTime:
                            var disbursementTime = CalculateOxygenDistributionTime(tiles);
                            cts.Cancel();
                            return disbursementTime;
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

        private async Task<(long X, long Y)> Explore(
            (long X, long Y) position,
            Queue<Direction> path,
            Channel<long, long> serverToMachineChannel,
            Channel<long, long> machineToServerChannel,
            Dictionary<(long X, long Y),ExploredTile> tiles,
            CancellationToken token)
        {
            Debug.Assert(path != null, nameof(path) + " != null");
            var nextDirection = (long)path.Dequeue();
            await serverToMachineChannel.Writer.WriteAsync(nextDirection, token);
            var cellType = await machineToServerChannel.Reader.ReadAsync(token);
            switch (cellType)
            {
                case 0:
                    AddTile(position.Move((Direction)nextDirection), tiles,
                        TileType.Wall);
                    path.Clear();
                    break;
                case 1:
                    position = position.Move((Direction)nextDirection);
                    AddTile(position, tiles, TileType.Open);
                    break;
                case 2:
                    position = position.Move((Direction)nextDirection);
                    AddTile(position, tiles, TileType.OxygenSystem);
                    break;
            }

            return position;
        }

        private static int CalculateOxygenDistributionTime(IReadOnlyDictionary<(long X, long Y), ExploredTile> tiles)
        {
            var oxygenTile = tiles.First(t => t.Value.Type == TileType.OxygenSystem);
            var distributionTime = PathingUtility.DijkstraLongestPath(oxygenTile.Key, tiles);
            return distributionTime;
        }

        private static void AddTile((long X, long Y) tilePosition, Dictionary<(long X, long Y), ExploredTile> tiles,
            TileType tile)
        {
            if (tiles.ContainsKey(tilePosition))
            {
                return;
            }

            tiles[tilePosition] = ExploredTile.Create(tilePosition, tile, tiles);
            foreach (var (position, direction) in tilePosition.GetAdjacentPositions())
            {
                if (tiles.TryGetValue(position, out var neighbourCell))
                {
                    neighbourCell.SetNeighbour(direction.Reverse(), tile);
                }
            }
        }

        private static Direction[] DecideMovement((long X, long Y) position, Dictionary<(long X, long Y), ExploredTile> tiles)
        {
            var current = tiles[position];
            if (current.HasUnexploredNeighbours)
            {
                var direction = current.UnexploredNeighbours.First();
                var queue = new[]
                {
                    direction,
                    direction.Reverse()
                };

                return queue;
            }

            return JourneyToNearestEmpty(position, tiles);
        }

        private static Direction[] JourneyToNearestEmpty((long X, long Y) position, IReadOnlyDictionary<(long X, long Y), ExploredTile> tiles)
        {
            return PathingUtility.DijkstraSearch(position, (t) => t.HasUnexploredNeighbours, tiles);
        }
    }
}
