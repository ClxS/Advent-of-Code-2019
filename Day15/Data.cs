namespace Day15
{
    using System.Collections.Generic;

    public class Data
    {
        public Data(string opCodes, bool printMap, IEnumerable<DroidState> states)
        {
            this.OpCodes = opCodes;
            this.PrintMap = printMap;
            this.States = new Queue<DroidState>(states);
        }

        public string OpCodes { get; }

        public bool PrintMap { get; }

        public Queue<DroidState> States { get; }
    }
}
