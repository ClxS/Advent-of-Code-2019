namespace CommonCode.Machine.DefaultOps
{
    using System;

    public class Multiply : IOp
    {
        public int DataLength => 3;

        public bool Act(ReadOnlySpan<int> opData, Memory<int> memory)
        {
            memory.Span[opData[2]] = memory.Span[opData[0]] * memory.Span[opData[1]];
            return true;
        }
    }
}
