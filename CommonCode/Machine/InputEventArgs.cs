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
                this.ValueAsync = Task.FromResult(this.value);
            }
        }

        public Task<long> ValueAsync { get; set; }

        public InputEventArgs()
        {
        }
    }
}
