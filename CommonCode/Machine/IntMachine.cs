namespace CommonCode.Machine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
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

        public event EventHandler Completed;
        
        public bool EnableExtendedOpCodeSupport { get; set; }

        public int Id { get; set; }

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

            this.Completed?.Invoke(null, EventArgs.Empty);
            return state;
        }

        public async Task<MachineState> ProcessAsync(int[] data)
        {
            this.memory = data.ToArray();
            var state = new MachineState(this.memory);
            var dataPivot = this.memory;
            this.readPivot = 0;
            while (!this.breakFlag)
            {
                var (op, opData, modeInfo) = this.GetOpCodeAndMode(dataPivot);
                switch (op)
                {
                    case IAsyncOp asyncOp:
                        await asyncOp.Act(this, opData, modeInfo);
                        break;
                    default:
                        op.Act(this, opData, modeInfo);
                        break;
                }

                if (!this.jumpFlag)
                {
                    this.readPivot += op.DataLength + 1;
                }

                dataPivot = this.memory.Slice(this.readPivot);
                this.jumpFlag = false;
            }

            this.Completed?.Invoke(null, EventArgs.Empty);
            return state;
        }

        private (IOp Op, int[] data, byte[] Modes) GetOpCodeAndMode(Memory<int> dataPivot)
        {
            Span<byte> modeInfoBuffer = stackalloc byte[16];
            Span<byte> componentsBuffer = stackalloc byte[16];
            var span = dataPivot.Span;
            var elements = span[0].DecomposeInt(componentsBuffer);
            var opData = componentsBuffer.Slice(0, elements);
            var opCode = elements == 1 ? opData[0] : opData[^2] * 10 + opData[^1];

            var op = this.ops[opCode];
            var modeInfo = modeInfoBuffer.Slice(0, op.DataLength);
            modeInfo.Fill(0);
            var modeSpan = opData.Slice(0, Math.Max(0, elements - 2));
            modeSpan.Reverse();
            modeSpan.CopyTo(modeInfo);

            return (op, span.Slice(1, op.DataLength).ToArray(), modeInfo.ToArray().Take(op.DataLength).ToArray());
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

        internal async Task<int> RequestOutputAsync()
        {
            var args = new InputEventArgs();
            this.InputRequested?.Invoke(null, args);
            return await args.ValueAsync;
        }

        internal int RequestOutput()
        {
            var args = new InputEventArgs();
            this.InputRequested?.Invoke(null, args);
            if (!args.IsSynchronous)
            {
                throw new Exception("Synchronous input requested. This should be set using InputEventArgs.Value");
            }

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
