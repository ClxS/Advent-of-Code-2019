namespace Day16
{
    public class Data
    {
        public Data(char[] number, int phases, bool useOffsetValue)
        {
            this.Number = number;
            this.Phases = phases;
            this.UseOffsetValue = useOffsetValue;
        }

        public char[] Number { get; }

        public int Phases { get; }

        public bool UseOffsetValue { get; }
    }
}
