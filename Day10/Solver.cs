namespace Day10
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Solver : ISolver
    {
        public (int X, int Y, int VisibleCount) Solve(Data inputData)
        {
            var asteroidPositions = new List<(int X, int Y)>();
            for (var y = 0; y < inputData.Map.Length; y++)
            {
                for (var x = 0; x < inputData.Map[y].Length; x++)
                {
                    if (inputData.Map[y][x] == '#')
                    {
                        asteroidPositions.Add((x, y));
                    }
                }
            }

            var asteroidVisibilities = asteroidPositions.Select(p => (p.X, p.Y, this.GetAsteroidsInLineOfSight(p, asteroidPositions)));
            var bestDescendingAsteroidVisibilities = asteroidVisibilities.OrderByDescending(v => v.Item3);

            return bestDescendingAsteroidVisibilities.First();
        }

        private int GetAsteroidsInLineOfSight((int X, int Y) pos, List<(int X, int Y)> asteroidPositions)
        {
            var leftAsteroids = asteroidPositions.Where(p => p.X <= pos.X && p != pos);
            var rightAsteroids = asteroidPositions.Where(p => p.X > pos.X && p != pos);

            var visibleLeftDotProducts = new HashSet<double>();
            foreach (var asteroid in leftAsteroids)
            {
                var dotProduct = this.GetAsteroidDotProduct(pos, asteroid);
                if (visibleLeftDotProducts.All(other => Math.Abs(dotProduct - other) > 0.00000001))
                {
                    visibleLeftDotProducts.Add(dotProduct);
                }
            }

            var visibleRightDotProducts = new HashSet<double>();
            foreach (var asteroid in rightAsteroids)
            {
                var dotProduct = this.GetAsteroidDotProduct(pos, asteroid);
                if (visibleRightDotProducts.All(other => Math.Abs(dotProduct - other) > 0.00000001))
                {
                    visibleRightDotProducts.Add(dotProduct);
                }
            }

            return visibleLeftDotProducts.Count + visibleRightDotProducts.Count;
        }

        private double GetAsteroidDotProduct((int X, int Y) pos, (int X, int Y) asteroid)
        {
            var (x, y) = pos;
            var (asX, asY) = asteroid;
            var (u, v) = (asX - x, asY - y);
            var magnitude = Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2));
            var (nU, nV) = (u / magnitude, v / magnitude);

            var (refX, refY) = (0, 1);
            return refX * nU + refY * nV;
        }
    }
}
