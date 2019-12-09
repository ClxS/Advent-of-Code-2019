namespace CommonCode.Machine.DefaultOps
{
    using System;
    using System.Threading.Tasks;

    public class Input : IAsyncOp
    { 
        public int DataLength => 1;

        void IOp.Act(IntMachine machine, ReadOnlySpan<int> opData, ReadOnlySpan<byte> modes)
        {
            var input = machine.RequestOutput();
            machine.Write(opData[0], input);
        }

        public async Task Act(IntMachine machine, int[] opData, byte[] modes)
        {
            var input = await machine.RequestOutputAsync();
            machine.Write(opData[0], input);
        }
    }
}
