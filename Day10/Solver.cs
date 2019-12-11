namespace Day10
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    internal class Solver : ISolver
    {
        public (int X, int Y, int VisibleCount) Solve(Data inputData)
        {
            var asteroidPositions = this.GetAsteroids(inputData.Map).ToArray();
            var asteroidVisibilities = asteroidPositions.Select(p => (p.X, p.Y, this.GetAsteroidsInLineOfSight(p, asteroidPositions).Count()));
            var bestDescendingAsteroidVisibilities = asteroidVisibilities.OrderByDescending(v => v.Item3);

            return bestDescendingAsteroidVisibilities.First();
        }

        public int Solve2(Data inputData)
        {
            var asteroidPositions = this.GetAsteroids(inputData.Map).ToArray();

            var remainingCount = inputData.RemoveCount;
            while (remainingCount > 0)
            {
                var asteroidsInSight = this.GetAsteroidsInLineOfSight(inputData.BestPosition, asteroidPositions).ToArray();

                if (asteroidsInSight.Length < inputData.RemoveCount)
                {
                    remainingCount -= asteroidsInSight.Length;
                    asteroidPositions = asteroidPositions
                                        .Where(p => !asteroidsInSight.Any(a => a.X == p.X && a.Y == p.Y)).ToArray();
                }
                else
                {
                    var orderedAsteroids = asteroidsInSight.OrderBy(a => a.Angle).ToArray();
                    var finalAsteroid = orderedAsteroids.ElementAt(remainingCount - 1);
                    //this.Print(inputData.Map, inputData.BestPosition, orderedAsteroids);
                    return finalAsteroid.X * 100 + finalAsteroid.Y;
                }
            }

            return 0;
        }

        private void ModifyChar((int X, int Y) pos, char newChar, string[] map)
        {
            var newLineChars = map[pos.Y].ToCharArray();
            newLineChars[pos.X] = newChar;
            map[pos.Y] = new string(newLineChars);
        }

        private void Print(string[] map, (int X, int Y) pos, (int X, int Y, double Angle, int Distance)[] orderedAsteroids)
        {
            this.ModifyChar(pos, '~', map);
            for (int i = 0; i < 200; i++)
            {
                this.ModifyChar((orderedAsteroids[i].X, orderedAsteroids[i].Y), '°', map);
                Debug.WriteLine($"\n\n\n======================================= {i}\n" +
                                $"{string.Join('\n', map)}" +
                                $"\n======================================= \n");
                Thread.Sleep(50);
            }
        }

        private IEnumerable<(int X, int Y)> GetAsteroids(string[] map)
        {
            for (var y = 0; y < map.Length; y++)
            {
                for (var x = 0; x < map[y].Length; x++)
                {
                    if (map[y][x] == '#')
                    {
                        yield return (x, y);
                    }
                }
            }
        }

        private static double DotProductToDegrees(double dotProduct, int semiCircle)
        {
            if (semiCircle > 1 || semiCircle < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (semiCircle == 0)
            {
                return 360.0 - Math.Acos(dotProduct) * (180.0 / Math.PI);
            }

            return Math.Acos(dotProduct) * (180.0 / Math.PI);
        }

        private IEnumerable<(int X, int Y, double Angle, int Distance)> GetAsteroidsInLineOfSight((int X, int Y) pos, (int X, int Y)[] asteroidPositions)
        {
            var leftAsteroids = asteroidPositions.Where(p => p.X < pos.X && p != pos);
            var rightAsteroids = asteroidPositions.Where(p => p.X >= pos.X && p != pos);

            var positions = new List<(int X, int Y, double Angle, int Distance)>();
            ProcessSet(0, pos, leftAsteroids, positions);
            ProcessSet(1, pos, rightAsteroids, positions);

            var orderedPositions = positions.OrderBy(p => p.Angle).ToArray();

            var groupedPositions = orderedPositions.GroupBy(p => (int)(p.Angle * 1000));
            var orderedGroups = groupedPositions.OrderBy(g => g.Key);

            return orderedGroups.Select(@group => @group.OrderBy(a => a.Distance).First()).ToList();
        }

        private static void ProcessSet(int set, (int X, int Y) pos, IEnumerable<(int X, int Y)> asteroidPositions, List<(int X, int Y, double Angle, int Distance)> asteroids)
        {
            foreach (var asteroid in asteroidPositions)
            {
                var dotProduct = GetAsteroidDotProduct(pos, asteroid);
                asteroids.Add((asteroid.X, asteroid.Y, DotProductToDegrees(dotProduct, set), ManhattenDistance(pos, asteroid)));
            }
        }

        private static int ManhattenDistance((int X, int Y) pos, (int X, int Y) asteroid)
        {
            return Math.Abs(pos.X - asteroid.X) + Math.Abs(pos.Y - asteroid.Y);
        }

        private static double GetAsteroidDotProduct((int X, int Y) pos, (int X, int Y) asteroid)
        {
            var (x, y) = pos;
            var (asX, asY) = asteroid;
            var (u, v) = (asX - x, asY - y);
            var magnitude = Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2));
            var (nU, nV) = (u / magnitude, v / magnitude);

            var (refX, refY) = (0, -1);
            return refX * nU + refY * nV;
        }
    }
}
