namespace Day4
{
    using System;
    using System.Collections.Generic;

    public class NaiveSolver : ISolver
    {
        private readonly bool stopRepeatedGroups;

        public NaiveSolver(bool stopRepeatedGroups)
        {
            this.stopRepeatedGroups = stopRepeatedGroups;
        }

        public int Solve(Data inputData)
        {
            var matchCount = 0;
            var digits = DecomposeInt(inputData.StartValue);
            for (var i = 0; i <= inputData.EndValue - inputData.StartValue; i++)
            {
                if (MatchesAdjacentDigitRequirement(digits) && MatchesNeverDecreaseValueRequirement(digits))
                {
                    matchCount++;
                }

                Increment(digits);
            }

            return matchCount;
        }

        private static bool MatchesNeverDecreaseValueRequirement(IEnumerable<byte> digits)
        {
            byte current = 0;
            foreach (var digit in digits)
            {
                if (current > digit)
                {
                    return false;
                }

                current = digit;
            }

            return true;
        }

        private bool MatchesAdjacentDigitRequirement(byte[] digits)
        {
            for (var i = 0; i < digits.Length - 1; i++)
            {
                if (digits[i] == digits[i + 1] && (!this.stopRepeatedGroups || ((i >= digits.Length - 2 || digits[i + 2] != digits[i]) && (i == 0 || digits[i - 1] != digits[i]))))
                {
                    return true;
                }
            }

            return false;
        }

        private static byte[] DecomposeInt(int value)
        {
            var output = new Stack<byte>();

            while (value > 0)
            {
                var digit = (byte)(value % 10);
                value /= 10;
                output.Push(digit);
            }

            return output.ToArray();
        }

        private static void Increment(IList<byte> val)
        {
            var idx = val.Count - 1;
            while (true)
            {
                if (idx < 0)
                {
                    throw new OverflowException();
                }

                if (val[idx] < 9)
                {
                    val[idx]++;
                    break;
                }

                val[idx] = 0;
                idx--;
            }
        }
    }
}
