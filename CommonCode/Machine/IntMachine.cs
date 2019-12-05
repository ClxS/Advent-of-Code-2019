namespace CommonCode.Machine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class IntMachine
    {
        private readonly Dictionary<int, IOp> ops;

        public IntMachine(params (int OpCode, IOp Operation)[] supportOpCodes)
        {
            this.ops = supportOpCodes.ToDictionary(v => v.OpCode, v => v.Operation);
        }

        public MachineState Process(int[] data)
        {
            Memory<int> memory = data;
            var state = new MachineState(memory);
            var dataPivot = memory.Span;
            var readPivot = 0;
            while (true)
            {
                var op = this.ops[dataPivot[0]];
                if (op.Act(dataPivot.Slice(1, op.DataLength), memory))
                {
                    readPivot += op.DataLength + 1;
                    dataPivot = memory.Slice(readPivot).Span;
                }
                else
                {
                    break;
                }
            }

            return state;
        }

        public class MachineState
        {
            public MachineState(Memory<int> memory)
            {
                this.Memory = memory;
            }

            public Memory<int> Memory { get; }
        }
    }

    public interface IOp
    {
        int DataLength { get; }

        bool Act(ReadOnlySpan<int> opData, Memory<int> memory);
    }
}
