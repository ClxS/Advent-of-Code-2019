namespace Day15.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;

    using TileSet = System.Collections.Generic.IReadOnlyDictionary<(long X, long Y), ExploredTile>;

    public static class PathingUtility
    {
        public static int DijkstraLongestPath(
            (long X, long Y) start,
            TileSet tiles)
        {
            var encounteredCells = new HashSet<(long, long)>();

            var pathQueues = new Queue<(Direction[] Path, (long X, long Y) EndPoint)>();
            foreach (var direction in tiles[start].OpenNeighbours)
            {
                pathQueues.Enqueue((new[] { direction }, start.Move(direction)));
            }

            var longestPath = 0;
            while (pathQueues.Count > 0)
            {
                var (path, endPoint) = pathQueues.Dequeue();
                if (!encounteredCells.Add(endPoint))
                {
                    continue;
                }

                if (path.Length > longestPath)
                {
                    longestPath = path.Length;
                }

                var cell = tiles[endPoint];
                foreach (var direction in cell.OpenNeighbours)
                {
                    pathQueues.Enqueue((path.Concat(new[] { direction }).ToArray(), endPoint.Move(direction)));
                }
            }

            return longestPath;
        }

        public static Direction[] DijkstraSearch(
            (long X, long Y) start,
            Func<ExploredTile, bool> searchCondition,
            TileSet tiles)
        {
            var encounteredCells = new HashSet<(long, long)>();

            var pathQueues = new Queue<(Direction[] Path, (long X, long Y) EndPoint)>();
            foreach (var direction in tiles[start].OpenNeighbours)
            {
                pathQueues.Enqueue((new[] { direction }, start.Move(direction)));
            }

            while (pathQueues.Count > 0)
            {
                var (path, endPoint) = pathQueues.Dequeue();
                if (!encounteredCells.Add(endPoint))
                {
                    continue;
                }

                var cell = tiles[endPoint];
                if (searchCondition(cell))
                {
                    return path;
                }

                foreach (var direction in cell.OpenNeighbours)
                {
                    pathQueues.Enqueue((path.Concat(new[] { direction }).ToArray(), endPoint.Move(direction)));
                }
            }

            return null;
        }
    }
}
