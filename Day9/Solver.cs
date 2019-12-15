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
            var intMachine = IntMachine.CreateDefault();

            var output = new List<long>();
            intMachine.InputRequested += (sender, args) =>
            {
                args.Value = inputData.InputValue;
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
