namespace CommonCode.Machine.DefaultOps
{
    using System;

    public class Input : IOp
    {
        public int DataLength => 1;

        public bool Act(ReadOnlySpan<int> opData, ReadOnlySpan<byte> modeIndicators, Memory<int> memory)
        {
            var input = IntMachine.RequestOutput();
            memory.Span[opData[0]] = input;
            return true;
        }
    }
}
