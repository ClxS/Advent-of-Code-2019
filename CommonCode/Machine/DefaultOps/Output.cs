using System;
using System.Collections.Generic;
using System.Text;

namespace CommonCode.Machine.DefaultOps
{
    public class Output : IOp
    {
        public int DataLength => 1;

        public bool Act(ReadOnlySpan<int> opData, ReadOnlySpan<byte> modeIndicators, Memory<int> memory)
        {
            IntMachine.SignalOutput(memory.Span[opData[0]]);
            return true;
        }
    }
}
