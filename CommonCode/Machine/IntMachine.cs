namespace CommonCode.Machine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DefaultOps;
    using Utility.Extensions;
    using Utility.Extentions;

    public class IntMachine
    {
        private readonly Dictionary<long, IOp> ops;
        private bool breakFlag;
        private bool jumpFlag;
        private Memory<long> memory;
        private int readPivot;
        private int relativeBase;

        public static IntMachine CreateDefault()
        {
            return new IntMachine(
                (1, new Add()),
                (2, new Multiply()),
                (3, new Input()),
                (4, new Output()),
                (5, new JmpTrue()),
                (6, new JmpFalse()),
                (7, new LessThan()),
                (8, new Equals()),
                (9, new OffsetRelativeBase()),
                (99, new Break())
            )
            {
                EnableExtendedOpCodeSupport = true,
                MinimumBufferSize = 32 * 1024
            };
        }

        public IntMachine(params (int OpCode, IOp Operation)[] supportOpCodes)
        {
            this.ops = supportOpCodes.ToDictionary(v => (long)v.OpCode, v => v.Operation);
            this.EnableExtendedOpCodeSupport = false;
        }

        public IntMachine(params (long OpCode, IOp Operation)[] supportOpCodes)
        {
            this.ops = supportOpCodes.ToDictionary(v => v.OpCode, v => v.Operation);
            this.EnableExtendedOpCodeSupport = false;
        }

        public event EventHandler<InputEventArgs> InputRequested;

        public event EventHandler<OutputEventArgs> Output;

        public event EventHandler Completed;
        
        public bool EnableExtendedOpCodeSupport { get; set; }

        public int MinimumBufferSize { get; set; }

        public int Id { get; set; }

        public void Break()
        {
            this.breakFlag = true;
        }

        public void Jump(long address)
        {
            this.jumpFlag = true;
            this.readPivot = (int)address;
        }

        public MachineState Process(int[] data)
        {
            return this.Process(data.Select(d => (long)d).ToArray());
        }

        public MachineState Process(long[] data)
        {
            this.memory = new long[Math.Max(data.Length, this.MinimumBufferSize)];
            data.CopyTo(this.memory);

            var state = new MachineState(this.memory);
            var dataPivot = this.memory.Span;
            this.readPivot = 0;
            this.relativeBase = 0;
            Span<byte> modeInfoBuffer = stackalloc byte[16];
            Span<byte> componentsBuffer = stackalloc byte[16];
            while (!this.breakFlag)
            {
                IOp op;
                if (this.EnableExtendedOpCodeSupport)
                {
                    var elements = dataPivot[0].DecomposeLong(componentsBuffer);
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

        public async Task<MachineState> ProcessAsync(int[] data, CancellationToken cancellationToken = default)
        {
            return await this.ProcessAsync(data.Select(d => (long)d).ToArray(), cancellationToken);
        }

        public async Task<MachineState> ProcessAsync(long[] data, CancellationToken cancellationToken = default)
        {
            this.memory = new long[Math.Max(data.Length, this.MinimumBufferSize)];
            data.CopyTo(this.memory);

            var state = new MachineState(this.memory);
            var dataPivot = this.memory;
            this.readPivot = 0;
            while (!this.breakFlag && !cancellationToken.IsCancellationRequested)
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

        private (IOp Op, long[] data, byte[] Modes) GetOpCodeAndMode(Memory<long> dataPivot)
        {
            Span<byte> modeInfoBuffer = stackalloc byte[16];
            Span<byte> componentsBuffer = stackalloc byte[16];
            var span = dataPivot.Span;
            var elements = span[0].DecomposeLong(componentsBuffer);
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

        public void Write(long address, int mode, long value)
        {
            this.memory.Span[this.GetWriteAddress(address, mode)] = value;
        }

        internal long MarshallAccess(long value, int mode)
        {
            switch (mode)
            {
                case 0: return this.memory.Span[(int)value];
                case 1: return value;
                case 2: return this.memory.Span[(int)(this.relativeBase + value)];
                default: throw new ArgumentOutOfRangeException();
            }
        }

        internal int GetWriteAddress(long value, int mode)
        {
            switch (mode)
            {
                case 0: return (int)value;
                case 2: return (int)(this.relativeBase + value);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        internal void OffsetRelativeBase(long address)
        {
            this.relativeBase += (int)address;
        }

        internal async Task<long> RequestOutputAsync()
        {
            var args = new InputEventArgs();
            this.InputRequested?.Invoke(null, args);
            return await args.ValueAsync;
        }

        internal long RequestOutput()
        {
            var args = new InputEventArgs();
            this.InputRequested?.Invoke(null, args);
            if (!args.IsSynchronous)
            {
                throw new Exception("Synchronous input requested. This should be set using InputEventArgs.Value");
            }

            return args.Value;
        }

        internal void SignalOutput(long output)
        {
            this.Output?.Invoke(null, new OutputEventArgs(output));
        }

        public class MachineState
        {
            public MachineState(Memory<long> memory)
            {
                this.Memory = memory;
            }

            public Memory<long> Memory { get; }
        }
    }
}
