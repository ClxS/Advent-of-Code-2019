﻿namespace Day7
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
        public async Task<int> Solve(Data inputData)
        {
            var data = inputData.OpCodes.Split(',').Select(int.Parse).ToArray();

            var largestValue = 0;
            foreach(var phase in GetAvailableThrusterCombinations(inputData.MinimumPhaseSettings, inputData.MaximumPhaseSettings))
            {
                var value = await this.RunWithPhaseSettings(data, phase.Select(x => (int)x), inputData.UseLoopback).ConfigureAwait(false);
                if (value > largestValue)
                {
                    largestValue = value;
                }
            }

            return largestValue;
        }

        private static IEnumerable<IEnumerable<byte>> GetAvailableThrusterCombinations(int min, int max)
        {
            return GetPermutations(Enumerable.Range(min, max - min + 1).Select(x => (byte)x));
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

            var values = new List<int>();
            await Task.Run(async () =>
              {
                  while (await targetChannel.Reader.WaitToReadAsync())
                  {
                      var lastValue = await targetChannel.Reader.ReadAsync();
                      values.Add(lastValue);
                      if (useLoopback)
                      {
                          await rootChannel.Writer.WriteAsync(lastValue);
                      }
                  }
              }).ConfigureAwait(false);

            await Task.WhenAll(machineTasks).ConfigureAwait(false);
            
            return values.Max();
        }

        private IntMachine CreateIntMachine(Channel<int> inputCommChannel, Channel<int> outputCommChannel, int machineIdx)
        {
            var intMachine = IntMachine.CreateDefault();
            intMachine.InputRequested += (sender, args) =>
            {
                args.ValueAsync = new ValueTask<long>(Task.Run(async () =>
                {
                    var value = await inputCommChannel.Reader.ReadAsync();
                    return (long)value;
                }));
            };
            intMachine.Output += (sender, args) =>
            {
                _ = outputCommChannel.Writer.WriteAsync((int)args.Output);
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
