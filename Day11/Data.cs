namespace Day11
{
    public class Data
    {
        public Data(string opCodes, long startColor, bool outputPaintedValue)
        {
            this.OpCodes = opCodes;
            this.StartColor = startColor;
            this.OutputPaintedValue = outputPaintedValue;
        }

        public string OpCodes { get; }

        public long StartColor { get; }

        public bool OutputPaintedValue { get; }
    }
}
