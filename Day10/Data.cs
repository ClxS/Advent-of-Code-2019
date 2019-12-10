namespace Day10
{
    public class Data
    {
        public Data(string[] map)
        {
            this.Map = map;
        }


        public Data(string[] map, (int X, int Y) bestPosition)
        {
            this.Map = map;
            this.BestPosition = bestPosition;
        }

        public string[] Map { get; }

        public (int X, int Y) BestPosition { get; }
    }
}
