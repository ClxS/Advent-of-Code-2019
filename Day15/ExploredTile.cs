namespace Day15
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;

    public class ExploredTile
    {
        public bool HasUnexploredNeighbours => this.UnexploredNeighbours.Any();

        public TileType[] Neighbours { get; private set; }

        public IEnumerable<Direction> OpenNeighbours
        {
            get
            {
                if (this.Neighbours[(int)Direction.North - 1] == TileType.Open ||
                    (this.Neighbours[(int)Direction.North - 1] == TileType.OxygenSystem))
                {
                    yield return Direction.North;
                }

                if (this.Neighbours[(int)Direction.South - 1] == TileType.Open ||
                    (this.Neighbours[(int)Direction.South - 1] == TileType.OxygenSystem))
                {
                    yield return Direction.South;
                }

                if (this.Neighbours[(int)Direction.West - 1] == TileType.Open ||
                    (this.Neighbours[(int)Direction.West - 1] == TileType.OxygenSystem))
                {
                    yield return Direction.West;
                }

                if (this.Neighbours[(int)Direction.East - 1] == TileType.Open ||
                    (this.Neighbours[(int)Direction.East - 1] == TileType.OxygenSystem))
                {
                    yield return Direction.East;
                }
            }
        }

        public (long X, long Y) Position { get; private set; }
        public TileType Type { get; private set; }

        public IEnumerable<Direction> UnexploredNeighbours
        {
            get
            {
                if (this.Neighbours[(int)Direction.North - 1] == TileType.Unknown)
                {
                    yield return Direction.North;
                }

                if (this.Neighbours[(int)Direction.South - 1] == TileType.Unknown)
                {
                    yield return Direction.South;
                }

                if (this.Neighbours[(int)Direction.West - 1] == TileType.Unknown)
                {
                    yield return Direction.West;
                }

                if (this.Neighbours[(int)Direction.East - 1] == TileType.Unknown)
                {
                    yield return Direction.East;
                }
            }
        }

        public static ExploredTile Create((long X, long Y) p, TileType type,
            Dictionary<(long X, long Y), ExploredTile> exploredTiles)
        {
            var northPos = p.Move(Direction.North);
            var southPos = p.Move(Direction.South);
            var westPos = p.Move(Direction.West);
            var eastPos = p.Move(Direction.East);

            var tile = new ExploredTile
            {
                Position = p,
                Type = type,
                Neighbours = new[]
                {
                    exploredTiles.ContainsKey(northPos)
                        ? exploredTiles[northPos].Type
                        : TileType.Unknown,
                    exploredTiles.ContainsKey(southPos)
                        ? exploredTiles[southPos].Type
                        : TileType.Unknown,
                    exploredTiles.ContainsKey(westPos)
                        ? exploredTiles[westPos].Type
                        : TileType.Unknown,
                    exploredTiles.ContainsKey(eastPos)
                        ? exploredTiles[eastPos].Type
                        : TileType.Unknown,
                }
            };

            return tile;
        }

        public void SetNeighbour(Direction direction, TileType tile)
        {
            this.Neighbours[(int)(direction) - 1] = tile;
        }
    }
}
