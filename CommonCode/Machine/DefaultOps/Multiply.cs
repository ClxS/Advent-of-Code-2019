namespace CommonCode.Machine.DefaultOps
{
    using System;

    public class Multiply : IOp
    {
        public int DataLength => 3;

        public bool Act(ReadOnlySpan<int> opData, ReadOnlySpan<byte> mode, Memory<int> memory)
        {
            memory.Span[opData[2]] = IntMachine.MarshallAccess(opData[0], mode[0], memory) *
                                     IntMachine.MarshallAccess(opData[1], mode[1], memory);
            return true;
        }
    }
}
