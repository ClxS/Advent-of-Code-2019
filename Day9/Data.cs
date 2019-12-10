namespace Day9
{
    public class Data
    {
        public Data(string opcodes, int inputValue)
        {
            this.OpCodes = opcodes;
            this.InputValue = inputValue;
        }

        public string OpCodes { get; }

        public int InputValue { get; }
    }
}
