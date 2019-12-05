namespace CommonCode.Machine
{
    using System;

    public class InputEventArgs : EventArgs
    {
        public int Value { get; set; }

        public InputEventArgs()
        {
        }
    }
}
