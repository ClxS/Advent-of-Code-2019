﻿namespace Day7
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Channels;
    using System.Threading.Tasks;
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

        public async Task<int> Solve(Data inputData)
        {
            var data = inputData.OpCodes.Split(',').Select(int.Parse).ToArray();

            var largestValue = 0;
            //foreach(var phase in GetAvailableThrusterCombinations(inputData.MinimumPhaseSettings, inputData.MaximumPhaseSettings))
            {
                var phase = new[] { 1, 0, 4, 3, 2 };
                var value = await this.RunWithPhaseSettings(data, phase.Select(x => (int)x), inputData.l);
                if (value > largestValue)
                {
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

        private async Task<int> RunWithPhaseSettings(int[] opCodes, IEnumerable<int> phaseValues, bool useLoopback)
        {
            var rootChannel = Channel.CreateUnbounded<int>();
            var amplifiers = this.CreateAmplifiers(phaseValues, rootChannel, out var targetChannel);
            await rootChannel.Writer.WriteAsync(0);

            var machineTasks = new List<Task>();
            foreach (var machine in amplifiers)
            {
                machineTasks.Add(machine.ProcessAsync(opCodes));
            }

            await Task.WhenAll(machineTasks);

            var lastValue = 0;
            while(await targetChannel.Reader.WaitToReadAsync())
            {
                lastValue = await targetChannel.Reader.ReadAsync();
            }

            return lastValue;
        }

        private const bool useSyncValue = false;

        private IntMachine CreateIntMachine(Channel<int> inputCommChannel, Channel<int> outputCommChannel, int machineIdx)
        {
            var intMachine = new IntMachine(this.intMachineSupportedCodes)
            {
                EnableExtendedOpCodeSupport = true,
                Id = machineIdx
            };
            intMachine.InputRequested += (sender, args) =>
            {
                args.ValueAsync = Task.Run(async () =>
                {
                    var value = await inputCommChannel.Reader.ReadAsync();
                    Debug.WriteLine($"Machine {machineIdx} - Read {value}");
                    return value;
                });
            };
            intMachine.Output += (sender, args) =>
            {
                Debug.WriteLine($"Machine {machineIdx} - Write {args.Output}");
                _ = outputCommChannel.Writer.WriteAsync(args.Output);
            };
            intMachine.Completed += (sender, args) =>
            {
                outputCommChannel.Writer.Complete();
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

        private IEnumerable<IntMachine> CreateAmplifiers(IEnumerable<int> phaseData, Channel<int> rootChannel, out Channel<int> destChannel)
        {
            destChannel = null;

            var amplifiers = new List<IntMachine>();
            var i = 0;

            foreach(var phase in phaseData)
            {
                destChannel = Channel.CreateUnbounded<int>();
                rootChannel.Writer.WriteAsync(phase);
                amplifiers.Add(this.CreateIntMachine(rootChannel, destChannel, i));
                rootChannel = destChannel;

                i++;
            }

            return amplifiers;
        }
    }
}
