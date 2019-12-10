namespace Day9
{
    using System.Collections.Generic;
    using System.Linq;
    using CommonCode.Machine;
    using CommonCode.Machine.DefaultOps;

    internal class Solver : ISolver
    {
        public string Solve(Data inputData)
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

            var output = new List<long>();
            intMachine.InputRequested += (sender, args) =>
            {
                args.Value = 1;
            };
            intMachine.Output += (sender, args) =>
            {
                output.Add(args.Output);
            };

            intMachine.Process(data);

            return string.Join(',', output);
        }
    }
}
