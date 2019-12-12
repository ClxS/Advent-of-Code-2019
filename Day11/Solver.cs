namespace Day11
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using CommonCode.Machine;
    using CommonCode.Machine.DefaultOps;

    internal class Solver : ISolver
    {
        public enum Color
        {
            Black = 0,
            White = 1
        }

        public (int Count, char[,] Pattern) Solve(Data inputData)
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

            var machineToServerChannel = Channel.CreateUnbounded<long>();
            var serverToMachineChannel = Channel.CreateUnbounded<long>();

            intMachine.InputRequested += (sender, args) =>
            {
                args.ValueAsync = serverToMachineChannel.Reader.ReadAsync();
            };
            intMachine.Output += (sender, args) => { machineToServerChannel.Writer.WriteAsync(args.Output); };
            intMachine.Completed += (sender, args) => { machineToServerChannel.Writer.Complete(); };

            serverToMachineChannel.Writer.WriteAsync(inputData.StartColor);
            var answerTask = Task.Run(async () =>
            {
                (int X, int Y) position = (0, 0);
                (int U, int V) forwardVec = (0, 1);
                var paintedCells = new Dictionary<(int X, int Y), Color>();
                while (await machineToServerChannel.Reader.WaitToReadAsync())
                {
                    var colour = (Color)await machineToServerChannel.Reader.ReadAsync();

                    // Paint the cell!
                    paintedCells[position] = colour;

                    var direction = await machineToServerChannel.Reader.ReadAsync();
                    switch (direction)
                    {
                        case 0:
                            forwardVec = this.RotateVector(forwardVec, 90);
                            break;
                        case 1:
                            forwardVec = this.RotateVector(forwardVec, -90);
                            break;
                    }

                    position = (position.X + forwardVec.U, position.Y + forwardVec.V);
                    await serverToMachineChannel.Writer.WriteAsync(this.GetCellColour(paintedCells, position));
                }

                return inputData.OutputPaintedValue
                    ? (paintedCells.Count, GetMap(paintedCells))
                    : (paintedCells.Count, (char[,])null);
            });

            Task.Run(async () => { await intMachine.ProcessAsync(data); });

            return answerTask.Result;
        }

        private static char[,] GetMap(Dictionary<(int X, int Y), Color> cells)
        {
            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var minY = int.MaxValue;
            var maxY = int.MinValue;
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
            for (var x = 0; x < output.GetLength(0); x++)
            {
                for (var y = 0; y < output.GetLength(1); y++)
                {
                    output[x, y] = '.';
                }
            }

            foreach (var position in cells)
            {
                output[position.Key.X - minX, height - (position.Key.Y - minY)] = position.Value == Color.White ? '@' : '.';
            }

            return output;
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private long GetCellColour(Dictionary<(int X, int Y), Color> paintedCells, (int X, int Y) position)
        {
            if (paintedCells.TryGetValue(position, out var colour))
            {
                return (long)colour;
            }

            return (long)Color.Black;
        }

        private (int U, int V) RotateVector((int U, int V) vector, int degrees)
        {
            var rads = this.DegreeToRadian(degrees);

            var (u, v) = vector;
            return ((int)(u * Math.Cos(rads) - v * Math.Sin(rads)),
                (int)(u * Math.Sin(rads) - v * Math.Cos(rads)));
        }
    }
}
