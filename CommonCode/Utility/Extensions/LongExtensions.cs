namespace CommonCode.Utility.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class LongExtensions
    {
        public static byte[] DecomposeLong(this long value)
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

        public static int DecomposeLong(this long value, Span<byte> outValue)
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
