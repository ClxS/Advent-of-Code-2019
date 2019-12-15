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
            var intMachine = IntMachine.CreateDefault();

            var output = 0;
            intMachine.InputRequested += (sender, args) => { args.Value = inputData.InputValue; };
            intMachine.Output += (sender, args) => { output = (int)args.Output; };

            var state = intMachine.Process(data);

            return output;
        }
    }
}
