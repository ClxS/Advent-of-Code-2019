namespace CommonCode.Machine
{
    using System;
    using System.Threading.Tasks;

    public class InputEventArgs : EventArgs
    {
        private int value;
        
        public bool IsSynchronous { get; private set; }

        public int Value
        {
            get => this.value;
            set
            {
                this.value = value;
                this.IsSynchronous = true;
                this.ValueAsync = Task.FromResult(this.value);
            }
        }

        public Task<int> ValueAsync { get; set; }

        public InputEventArgs()
        {
        }
    }
}
