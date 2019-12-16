namespace CommonCode.Utility.Extentions
{
    using System;
    using System.Collections.Generic;

    public static class IntExtensions
    {
        public static byte[] DecomposeInt(this int value)
        {
            if (value == 0)
            {
                return new byte[] { 0 };
            }

            var output = new Stack<byte>();
            while (value > 0)
            {
                var digit = (byte)(value % 10);
                value /= 10;
                output.Push(digit);
            }

            return output.ToArray();
        }

        public static int DecomposeInt(this int value, Span<byte> outValue)
        {
            var count = 0;
            while (value > 0)
            {
                count++;
                var digit = (byte)(value % 10);
                value /= 10;
                outValue[^count] = digit;
            }

            outValue.Slice(outValue.Length - count).CopyTo(outValue);
            return count;
        }
    }
}
