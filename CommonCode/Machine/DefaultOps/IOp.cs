namespace CommonCode.Machine
{
    using System;

    public interface IOp
    {
        int DataLength { get; }

        bool Act(ReadOnlySpan<int> opData, ReadOnlySpan<byte> modeIndicators, Memory<int> memory);
    }
}
