namespace Day13
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using CommonCode.Machine;
    using CommonCode.Machine.DefaultOps;

    internal class Solver : ISolver
    {
        public enum Tile
        {
            Empty = 0,
            Wall = 1,
            Block = 2,
            HorizontalPaddle = 3,
            Ball = 4
        }

        public (int BlocksRemaining, long Score) Solve(Data inputData)
        {
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

            long score = 0;
            var cellTiles = new Dictionary<(long X, long Y), Tile>();
            var serverToMachineChannel = Channel.CreateUnbounded<long>();
            var machineToServerChannel = Channel.CreateUnbounded<long>();
            var cts = new CancellationTokenSource();

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
                cts.Cancel();
            };

            var ballPosSemaphore = new SemaphoreSlim(0, 1);
            Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    await ballPosSemaphore.WaitAsync(cts.Token);
                    if (cts.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    while (true)
                    {
                        if (cellTiles.ContainsValue(Tile.Ball) && cellTiles.ContainsValue(Tile.HorizontalPaddle))
                        {
                            KeyValuePair<(long X, long Y), Tile> ballPos;
                            KeyValuePair<(long X, long Y), Tile> paddlePos;
                            lock (cellTiles)
                            {
                                ballPos = cellTiles.FirstOrDefault(v => v.Value == Tile.Ball);
                                paddlePos = cellTiles.FirstOrDefault(v => v.Value == Tile.HorizontalPaddle);
                            }

                            if (ballPos.Key.X > paddlePos.Key.X)
                            {
                                await serverToMachineChannel.Writer.WriteAsync(1, cts.Token);
                            }
                            else if (ballPos.Key.X < paddlePos.Key.X)
                            {
                                await serverToMachineChannel.Writer.WriteAsync(-1, cts.Token);
                            }
                            else
                            {
                                await serverToMachineChannel.Writer.WriteAsync(0, cts.Token);
                            }

                            break;
                        }

                        // We don't have a paddle position yet...
                        await Task.Delay(100, cts.Token);
                    }
                }
            }, cts.Token);

            var answerTask = Task.Run(async () =>
            {
                var populationOccured = false;
                (long X, long Y) position = (0, 0);
                while (await machineToServerChannel.Reader.WaitToReadAsync(cts.Token))
                {
                    position.X = await machineToServerChannel.Reader.ReadAsync(cts.Token);
                    position.Y = await machineToServerChannel.Reader.ReadAsync(cts.Token);

                    var movePaddle = false;
                    if (position.X == -1 && position.Y == 0)
                    {
                        score = await machineToServerChannel.Reader.ReadAsync(cts.Token);
                        populationOccured = true;
                    }
                    else
                    {
                        var tile = (Tile)await machineToServerChannel.Reader.ReadAsync(cts.Token);
                        lock (cellTiles)
                        {
                            cellTiles[position] = tile;
                        }

                        movePaddle = cellTiles[position] == Tile.Ball;
                    }

                    if (inputData.PrintMap)
                    {
                        this.PrintMap(GetMap(cellTiles), score);
                        await Task.Delay(10, cts.Token);
                    }

                    if (movePaddle)
                    {
                        ballPosSemaphore.Release();
                    }
                }

                return (cellTiles.Count(t => t.Value == Tile.Block), score);
            }, cts.Token);

            Task.Run(async () => { await intMachine.ProcessAsync(data); }, cts.Token);

            return answerTask.Result;
        }

        private static char[,] GetMap(Dictionary<(long X, long Y), Tile> cells)
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
        }
    }
}
