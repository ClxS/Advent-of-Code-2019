namespace CommonCode.Machine
{
    using System;
    using System.Threading.Tasks;

    public class InputEventArgs : EventArgs
    {
        private long value;
        
        public bool IsSynchronous { get; private set; }

        public long Value
        {
            get => this.value;
            set
            {
                this.value = value;
                this.IsSynchronous = true;
                this.ValueAsync = new ValueTask<long>(this.value);
            }
        }

        public ValueTask<long> ValueAsync { get; set; }

        public InputEventArgs()
        {
        }
    }
}
