namespace Day3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class PopCountSolver : ISolver
    {
        public enum Direction
        {
            Left,
            Up,
            Right,
            Down
        }

        public (int Distance, int Steps) Solve(Data inputData)
        {
            var wiresFromData = inputData.WireSpecifications.Select(Wire.Create);
            var collisions = wiresFromData.SelectMany(wire => wire.Path.Select(p => (wire, p.X, p.Y, p.Step, p.Dist)))
                                          .GroupBy(d => (d.X, d.Y))
                                          .Where(d => d.Count() > 1)
                                          .Select(g => new { Pos = g.Key, Dist = Math.Abs(g.Key.X) + Math.Abs(g.Key.Y), Collisions = g })
                                          .ToArray();

            var finalSum = int.MaxValue;
            foreach (var collision in collisions)
            {
                var lowestToReach = collision.Collisions
                                             .GroupBy(g => g.wire)
                                             .ToDictionary(g => g.Key, g => g.OrderBy(s => s.Step).First().Step);
                if (lowestToReach.Count <= 1)
                {
                    continue;
                }

                finalSum = Math.Min(finalSum, lowestToReach.Sum(k => k.Value));
            }

            return (collisions.OrderBy(c => c.Dist).First().Dist, finalSum);
        }

        public struct Movement
        {
            public Direction Direction;

            public int Magnitude;

            public Movement(Direction direction, int magnitude)
            {
                this.Direction = direction;
                this.Magnitude = magnitude;
            }

            public static Movement Create(string section)
            {
                Direction direction;
                switch (section[0])
                {
                    case 'L':
                        direction = Direction.Left;
                        break;
                    case 'U':
                        direction = Direction.Up;
                        break;
                    case 'R':
                        direction = Direction.Right;
                        break;
                    case 'D':
                        direction = Direction.Down;
                        break;
                    default: throw new IndexOutOfRangeException();
                }

                var magnitude = int.Parse(section.Skip(1).ToArray());
                return new Movement(direction, magnitude);
            }
        }

        public class Wire
        {
            private int x;

            private int y;

            private readonly List<(int X, int Y, int Step, int Dist)> path;

            protected Wire()
            {
                this.path = new List<(int X, int Y, int Step, int Dist)>();
            }
            
            public static Wire Create(string spec)
            {
                var wire = new Wire();
                var sections = spec.Split(',');
                foreach (var section in sections)
                {
                    wire.Move(Movement.Create(section));
                }

                return wire;
            }

            public IEnumerable<(int X, int Y, int Step, int Dist)> Path => this.path;

            private int step;

            protected void Move(Movement movement)
            {
                switch (movement.Direction)
                {
                    case Direction.Left:
                        for (var i = 0; i < movement.Magnitude; i++)
                        {
                            this.x--;
                            this.step++;
                            this.path.Add((this.x, this.y, this.step, Math.Abs(this.x) + Math.Abs(this.y)));
                        }
                        break;
                    case Direction.Up:
                        for (var i = 0; i < movement.Magnitude; i++)
                        {
                            this.y--;
                            this.step++;
                            this.path.Add((this.x, this.y, this.step, Math.Abs(this.x) + Math.Abs(this.y)));
                        }
                        break;
                    case Direction.Right:
                        for (var i = 0; i < movement.Magnitude; i++)
                        {
                            this.x++;
                            this.step++;
                            this.path.Add((this.x, this.y, this.step, Math.Abs(this.x) + Math.Abs(this.y)));
                        }
                        break;
                    case Direction.Down:
                        for (var i = 0; i < movement.Magnitude; i++)
                        {
                            this.y++;
                            this.step++;
                            this.path.Add((this.x, this.y, this.step, Math.Abs(this.x) + Math.Abs(this.y)));
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
        }
    }
}
