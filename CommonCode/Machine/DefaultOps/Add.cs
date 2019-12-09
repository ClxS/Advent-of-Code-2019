namespace CommonCode.Machine.DefaultOps
{
    using System;
    using System.Diagnostics;

    public class Add : IOp
    {
        public int DataLength => 3;

        public void Act(IntMachine machine, ReadOnlySpan<int> opData, ReadOnlySpan<byte> mode)
        {
            machine.Write(opData[2], machine.MarshallAccess(opData[0], mode[0]) + machine.MarshallAccess(opData[1], mode[1]));
        }
    }
}
