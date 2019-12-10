namespace CommonCode.Machine.DefaultOps
{
    using System;

    public class JmpTrue : IOp
    {
        public int DataLength => 2;

        public void Act(IntMachine machine, ReadOnlySpan<long> opData, ReadOnlySpan<byte> modes)
        {
            if (machine.MarshallAccess(opData[0], modes[0]) != 0)
            {
                machine.Jump(machine.MarshallAccess(opData[1], modes[1]));
            }
        }
    }
}
