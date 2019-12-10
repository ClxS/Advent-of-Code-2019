namespace CommonCode.Machine.DefaultOps
{
    using System;

    public class Multiply : IOp
    {
        public int DataLength => 3;

        public void Act(IntMachine machine, ReadOnlySpan<long> opData, ReadOnlySpan<byte> modes)
        {
            machine.Write(opData[2], modes[2], machine.MarshallAccess(opData[0], modes[0]) * machine.MarshallAccess(opData[1], modes[1]));
        }
    }
}
