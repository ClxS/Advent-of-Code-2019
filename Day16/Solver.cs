namespace Day16
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using CommonCode.Utility.Extensions;
    using CommonCode.Utility.Extentions;

    internal class Solver : ISolver
    {
        private class IterablePattern
        {
            private readonly int[] pattern;
            private readonly int startOffset;

            public IterablePattern(int[] pattern, int startOffset)
            {
                this.pattern = pattern;
                this.startOffset = startOffset;
            }

            public IEnumerable<long> GetForRow(int rowIndex)
            {
                var toPrint = rowIndex ;
                var idx = 0;
                while (true)
                {
                    for (var i = 0; i < toPrint; i++)
                    {
                        yield return this.pattern[this.SafeIndex(idx)];
                    }

                    toPrint = rowIndex + 1;
                    idx++;
                }
            }

            public void PopulateForRow(int rowIndex, long[] data)
            {
                var toPrint = rowIndex;
                var idx = 0;
                var outIdx = 0;
                while (outIdx < data.Length)
                {
                    var safeIdx = this.SafeIndex(idx);
                    for (var i = 0; i < toPrint; i++)
                    {
                        data[outIdx++] = this.pattern[safeIdx];
                        if (outIdx >= data.Length)
                        {
                            break;
                        }
                    }

                    toPrint = rowIndex + 1;
                    idx++;
                }
            }

            private int SafeIndex(int index)
            {
                if (index < 0)
                {
                    index = 4 + index;
                }

                return index % 4;
            }
        }

        public string Solve(Data inputData)
        {
            var pattern = new[] { 0, 1, 0, -1 };
            var numbers = inputData.Number.Select(n => (int)char.GetNumericValue(n)).ToArray();
            var parts = new int[numbers.Length];
            for (var phase = 0; phase < inputData.Phases; phase++)
            {
                for (var row = 0; row < numbers.Length; row++)
                {
                    var total = 0;
                    for (var col = row; col < numbers.Length; col++)
                    {
                        total += pattern[(1 + (col - row) / (1 + row)) % 4] * numbers[col];
                    }

                    parts[row] = total;
                }

                Parallel.For(0, parts.Length, i =>
                {
                    numbers[i] = Math.Abs(parts[i]).DecomposeInt().Last();
                });
            }

            var offset = inputData.UseOffsetValue ? ComposeNumber(inputData.Number.Take(7)) : 0;
            var output = new string(numbers.Skip(offset).Take(8).Select(c => Math.Abs(c).DecomposeInt().Last().ToString()[0]).ToArray());
            return new string(output);
        }

        private static int ComposeNumber(IEnumerable<char> digits)
        {
            return int.Parse(new string(digits.ToArray()));
        }
    }
}
