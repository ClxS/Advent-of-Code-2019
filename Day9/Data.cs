namespace Day9
{
    public class Data
    {
        public Data(string opcodes, int value)
        {
            this.OpCodes = opcodes;
            this.Value = value;
        }

        public string OpCodes { get; }

        public int Value { get; }
    }
}
