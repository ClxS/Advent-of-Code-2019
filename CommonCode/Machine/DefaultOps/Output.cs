namespace CommonCode.Machine.DefaultOps
{
    using System;

    public class Output : IOp
    {
        public int DataLength => 1;

        public void Act(IntMachine machine, ReadOnlySpan<int> opData, ReadOnlySpan<byte> mode)
        {
            machine.SignalOutput(machine.MarshallAccess(opData[0], mode[0]));
        }
    }
}
