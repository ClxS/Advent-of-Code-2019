namespace Day7
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using CommonCode.Machine;
    using CommonCode.Machine.DefaultOps;

    internal class Solver : ISolver
    {
        private readonly (int OpCode, IOp Operation)[] intMachineSupportedCodes;

        const int amplifierCount = 5;

        public Solver()
        {
            this.intMachineSupportedCodes = new (int OpCode, IOp Operation)[]
            {
                (1, new Add()),
                (2, new Multiply()),
                (3, new Input()),
                (4, new Output()),
                (5, new JmpTrue()),
                (6, new JmpFalse()),
                (7, new LessThan()),
                (8, new Equals()),
                (99, new Break())
            };
        }

        public int Solve(Data inputData)
        {
            var data = inputData.OpCodes.Split(',').Select(int.Parse).ToArray();

            int largestValue = 0;

            foreach(var phase in GetAvailableThrusterCombinations(inputData.MinimumPhaseSettings, inputData.MaximumPhaseSettings))
            {
                var reversedPhase = phase.Reverse().ToArray();
                if (!IsSuitablePhase(reversedPhase))
                {
                    continue;
                }
                
                var value = this.RunWithPhaseSettings(data, reversedPhase.Select(x => (int)x));
                if (value > largestValue)
                {
                    Debug.WriteLine($"Blah {string.Join(", ", phase)}");
                    largestValue = value;
                }
            }

            return largestValue;
        }

        private static IEnumerable<IEnumerable<byte>> GetAvailableThrusterCombinations(int min, int max)
        {
            return GetPermutations(Enumerable.Range(min, max + 1).Select(x => (byte)x));
        }

        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items)
        {
            var itemsArr = items as T[] ?? items.ToArray();

            // From https://stackoverflow.com/a/31201747
            foreach (var item in itemsArr)
            {
                var itemAsEnumerable = Enumerable.Repeat(item, 1).ToArray();
                var subSet = itemsArr.Except(itemAsEnumerable);
                if (!subSet.Any())
                {
                    yield return itemAsEnumerable;
                }
                else
                {
                    foreach (var sub in GetPermutations(itemsArr.Except(itemAsEnumerable)))
                    {
                        yield return itemAsEnumerable.Union(sub);
                    }
                }
            }
        }

        private static bool IsSuitablePhase(IEnumerable<byte> amplifierPhaseSettings)
        {
            return amplifierPhaseSettings.All(entry => entry <= maxPhase);
        }

        private int RunWithPhaseSettings(int[] opCodes, IEnumerable<int> phaseValues)
        {
            var phaseInputData = new Stack<int>(phaseValues);

            // Always use 0 as the second phase input for the first iteration
            PopInputSignal(phaseInputData, 0);

            var amplifiers = this.CreateAmplifiers(phaseInputData);
            foreach (var amplifier in amplifiers)
            {
                amplifier.Process(opCodes);
            }

            return phaseInputData.Pop();
        }

        private IntMachine CreateIntMachine(Stack<int> phaseData)
        {
            var intMachine = new IntMachine(this.intMachineSupportedCodes)
            {
                EnableExtendedOpCodeSupport = true
            };
            intMachine.InputRequested += (sender, args) =>
            {
                args.Value = phaseData.Pop();
            };
            intMachine.Output += (sender, args) =>
            {
                PopInputSignal(phaseData, args.Output);
            };

            return intMachine;
        }

        private static void PopInputSignal(Stack<int> phaseData, int value)
        {
            if (phaseData.Count == 0)
            {
                phaseData.Push(value);
            }
            else
            {
                var topValue = phaseData.Pop();
                phaseData.Push(value);
                phaseData.Push(topValue);
            }
        }

        private IEnumerable<IntMachine> CreateAmplifiers(Stack<int> phaseData)
        {
            var amplifiers = new List<IntMachine>();
            for (var amplifierIdx = 0; amplifierIdx < amplifierCount; ++amplifierIdx)
            {
                amplifiers.Add(this.CreateIntMachine(phaseData));
            }

            return amplifiers;
        }
    }
}
