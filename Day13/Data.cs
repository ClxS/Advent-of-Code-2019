namespace Day13
{
    public class Data
    {
        public Data(string opCodes, bool printMap)
        {
            this.OpCodes = opCodes;
            this.PrintMap = printMap;
        }

        public string OpCodes { get; }

        public bool PrintMap { get; }
    }
}
