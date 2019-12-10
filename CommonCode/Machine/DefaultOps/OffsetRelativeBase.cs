namespace CommonCode.Machine.DefaultOps
{
    using System;

    public class OffsetRelativeBase : IOp
    {
        public int DataLength => 1;

        public void Act(IntMachine machine, ReadOnlySpan<int> opData, ReadOnlySpan<byte> modes)
        {
            machine.OffsetRelativeBase(machine.MarshallAccess(opData[0], modes[0]));
        }
    }
}
