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
            this.EnableExtendedOpCodeSupport = false;
        }

        public static event EventHandler<InputEventArgs> InputRequested;

        public static event EventHandler<OutputEventArgs> Output;

        public bool EnableExtendedOpCodeSupport { get; set; }

        public MachineState Process(int[] data)
        {
            Memory<int> memory = data;
            var state = new MachineState(memory);
            var dataPivot = memory.Span;
            var readPivot = 0;
            Span<byte> modeInfoBuffer = stackalloc byte[16];
            Span<byte> componentsBuffer = stackalloc byte[16];
            while (true)
            {
                IOp op;
                bool canContinue;

                if (this.EnableExtendedOpCodeSupport)
                {
                    var elements = this.DecomposeInt(dataPivot[0], componentsBuffer);
                    var opData = componentsBuffer.Slice(0, elements);
                    var opCode = elements == 1 ? opData[0] : opData[^2] * 10 + opData[^1];

                    op = this.ops[opCode];
                    var modeInfo = modeInfoBuffer.Slice(0, op.DataLength);
                    modeInfo.Fill(0);
                    var modeSpan = opData.Slice(0, Math.Max(0, elements - 2));
                    modeSpan.Reverse();
                    modeSpan.CopyTo(modeInfo);

                    canContinue = op.Act(dataPivot.Slice(1, op.DataLength), modeInfo, memory);
                }
                else
                {
                    op = this.ops[dataPivot[0]];
                    var modeInfo = modeInfoBuffer.Slice(0, op.DataLength);
                    modeInfo.Fill(0);

                    canContinue = op.Act(dataPivot.Slice(1, op.DataLength), modeInfo, memory);
                }

                if (canContinue)
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

        internal static int MarshallAccess(int value, int mode, Memory<int> memory)
        {
            return mode == 0
                ? memory.Span[value]
                : value;
        }

        internal static int RequestOutput()
        {
            var args = new InputEventArgs();
            InputRequested?.Invoke(null, args);
            return args.Value;
        }

        internal static void SignalOutput(int output)
        {
            Output?.Invoke(null, new OutputEventArgs(output));
        }

        private int DecomposeInt(int value, Span<byte> outValue)
        {
            var count = 0;
            while (value > 0)
            {
                count++;
                var digit = (byte)(value % 10);
                value /= 10;
                outValue[^count] = digit;
            }

            outValue.Slice(outValue.Length - count).CopyTo(outValue);
            return count;
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
