namespace CommonCode.Machine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DefaultOps;
    using Utility.Extentions;

    public class IntMachine
    {
        private readonly Dictionary<int, IOp> ops;
        private bool breakFlag;
        private bool jumpFlag;
        private Memory<int> memory;
        private int readPivot;

        public IntMachine(params (int OpCode, IOp Operation)[] supportOpCodes)
        {
            this.ops = supportOpCodes.ToDictionary(v => v.OpCode, v => v.Operation);
            this.EnableExtendedOpCodeSupport = false;
        }

        public event EventHandler<InputEventArgs> InputRequested;

        public event EventHandler<OutputEventArgs> Output;

        public bool EnableExtendedOpCodeSupport { get; set; }

        public void Break()
        {
            this.breakFlag = true;
        }

        public void Jump(int address)
        {
            this.jumpFlag = true;
            this.readPivot = address;
        }

        public MachineState Process(int[] data)
        {
            this.memory = data;
            var state = new MachineState(this.memory);
            var dataPivot = this.memory.Span;
            this.readPivot = 0;
            Span<byte> modeInfoBuffer = stackalloc byte[16];
            Span<byte> componentsBuffer = stackalloc byte[16];
            while (!this.breakFlag)
            {
                IOp op;
                if (this.EnableExtendedOpCodeSupport)
                {
                    var elements = dataPivot[0].DecomposeInt(componentsBuffer);
                    var opData = componentsBuffer.Slice(0, elements);
                    var opCode = elements == 1 ? opData[0] : opData[^2] * 10 + opData[^1];

                    op = this.ops[opCode];
                    var modeInfo = modeInfoBuffer.Slice(0, op.DataLength);
                    modeInfo.Fill(0);
                    var modeSpan = opData.Slice(0, Math.Max(0, elements - 2));
                    modeSpan.Reverse();
                    modeSpan.CopyTo(modeInfo);

                    op.Act(this, dataPivot.Slice(1, op.DataLength), modeInfo);
                }
                else
                {
                    op = this.ops[dataPivot[0]];
                    var modeInfo = modeInfoBuffer.Slice(0, op.DataLength);
                    modeInfo.Fill(0);

                    op.Act(this, dataPivot.Slice(1, op.DataLength), modeInfo);
                }

                if (!this.jumpFlag)
                {
                    this.readPivot += op.DataLength + 1;
                }

                dataPivot = this.memory.Slice(this.readPivot).Span;
                this.jumpFlag = false;
            }

            return state;
        }

        public void Write(int address, int value)
        {
            this.memory.Span[address] = value;
        }

        internal int MarshallAccess(int value, int mode)
        {
            return mode == 0
                ? this.memory.Span[value]
                : value;
        }

        internal int RequestOutput()
        {
            var args = new InputEventArgs();
            this.InputRequested?.Invoke(null, args);
            return args.Value;
        }

        internal void SignalOutput(int output)
        {
            this.Output?.Invoke(null, new OutputEventArgs(output));
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
}
