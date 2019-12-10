namespace Day2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommonCode.Machine;
    using CommonCode.Machine.DefaultOps;

    internal class Solver : ISolver
    {
        public int Solve(Data inputData)
        {
            var data = inputData.OpCodes.Split(',').Select(int.Parse).ToArray();
            var intMachine = new IntMachine(
                (1, new Add()),
                (2, new Multiply()),
                (99, new Break())
            );

            var state = intMachine.Process(data);

            return (int)state.Memory.Span[0];
        }
    }
}
