namespace CommonCode.Machine.DefaultOps
{
    using System;

    public class Input : IOp
    {
        public int DataLength => 1;

        public void Act(IntMachine machine, ReadOnlySpan<int> opData, ReadOnlySpan<byte> modeIndicators)
        {
            var input = machine.RequestOutput();
            machine.Write(opData[0], input);
        }
    }
}
