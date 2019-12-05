namespace CommonCode.Machine.DefaultOps
{
    using System;

    public class Break : IOp
    {
        public int DataLength => 0;

        public void Act(IntMachine machine, ReadOnlySpan<int> opData, ReadOnlySpan<byte> modeIndicators)
        {
            machine.Break();
        }
    }
}
