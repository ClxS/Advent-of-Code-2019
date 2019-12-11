namespace Day10
{
    public class Data
    {
        public Data(string[] map)
        {
            this.Map = map;
        }


        public Data(string[] map, (int X, int Y) bestPosition, int removeCount)
        {
            this.Map = map;
            this.BestPosition = bestPosition;
            this.RemoveCount = removeCount;
        }

        public string[] Map { get; }

        public (int X, int Y) BestPosition { get; }

        public int RemoveCount { get; }
    }
}
