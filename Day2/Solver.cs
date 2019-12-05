namespace Day2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Solver : ISolver
    {
        private readonly Dictionary<int, IOp> ops = new Dictionary<int, IOp>()
        {
            { 1, new Add() },
            { 2, new Multiply() },
            { 99, new Break() }
        };

        public int Solve(Data inputData)
        {
            Memory<int> memory = inputData.OpCodes.Split(',').Select(int.Parse).ToArray();
            var data = memory.Span;

            var readPivot = 0;
            while (true)
            {
                var op = this.ops[data[0]];
                if (op.Act(data.Slice(1, op.DataLength), memory))
                {
                    readPivot += op.DataLength + 1;
                    data = memory.Slice(readPivot).Span;
                }
                else
                {
                    break;
                }
            }

            return memory.Span[0];
        }

        private interface IOp
        {
            int DataLength { get; }

            bool Act(ReadOnlySpan<int> opData, Memory<int> memory);
        }

        private class Add : IOp
        {
            public int DataLength => 3;

            public bool Act(ReadOnlySpan<int> opData, Memory<int> memory)
            {
                memory.Span[opData[2]] = memory.Span[opData[0]] + memory.Span[opData[1]];
                return true;
            }
        }

        private class Multiply : IOp
        {
            public int DataLength => 3;

            public bool Act(ReadOnlySpan<int> opData, Memory<int> memory)
            {
                memory.Span[opData[2]] = memory.Span[opData[0]] * memory.Span[opData[1]];
                return true;
            }
        }

        private class Break : IOp
        {
            public int DataLength => 0;

            public bool Act(ReadOnlySpan<int> opData, Memory<int> memory)
            {
                return false;
            }
        }
    }
}
