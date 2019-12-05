namespace CommonCode.Machine.DefaultOps
{
    using System;

    public interface IOp
    {
        int DataLength { get; }

        void Act(IntMachine machine, ReadOnlySpan<int> opData, ReadOnlySpan<byte> modes);
    }
}
