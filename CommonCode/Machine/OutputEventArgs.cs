namespace CommonCode.Machine
{
    using System;

    public class OutputEventArgs : EventArgs
    {
        public int Output { get; }

        public OutputEventArgs(int output)
        {
            this.Output = output;
        }
    }
}
