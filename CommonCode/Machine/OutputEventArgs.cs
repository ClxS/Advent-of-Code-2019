namespace CommonCode.Machine
{
    using System;

    public class OutputEventArgs : EventArgs
    {
        public long Output { get; }

        public OutputEventArgs(long output)
        {
            this.Output = output;
        }
    }
}
