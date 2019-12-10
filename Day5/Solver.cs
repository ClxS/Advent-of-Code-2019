namespace Day2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommonCode.Machine;
    using CommonCode.Machine.DefaultOps;
    using Xunit;

    internal class Solver : ISolver
    {
        public int Solve(Data inputData)
        {
            var data = inputData.OpCodes.Split(',').Select(int.Parse).ToArray();
            var intMachine = new IntMachine(
                (1, new Add()),
                (2, new Multiply()),
                (3, new Input()),
                (4, new Output()),
                (5, new JmpTrue()),
                (6, new JmpFalse()),
                (7, new LessThan()),
                (8, new Equals()),
                (99, new Break())
            )
            {
                EnableExtendedOpCodeSupport = true
            };

            var output = 0;
            intMachine.InputRequested += (sender, args) => { args.Value = inputData.InputValue; };
            intMachine.Output += (sender, args) => { output = (int)args.Output; };

            var state = intMachine.Process(data);

            return output;
        }
    }
}
