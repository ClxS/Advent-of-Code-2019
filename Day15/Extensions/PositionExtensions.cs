namespace Day15.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class PositionExtensions
    {
        public static (long X, long Y) Move(this (long X, long Y) @this, Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return (@this.X, @this.Y - 1);
                case Direction.South:
                    return (@this.X, @this.Y + 1);
                case Direction.West:
                    return (@this.X - 1, @this.Y);
                case Direction.East:
                    return (@this.X + 1, @this.Y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static IEnumerable<((long X, long Y) Position, Direction direction)> GetAdjacentPositions(
            this (long X, long Y) p)
        {
            yield return (p.Move(Direction.North), Direction.North);
            yield return (p.Move(Direction.South), Direction.South);
            yield return (p.Move(Direction.West), Direction.West);
            yield return (p.Move(Direction.East), Direction.East);
        }
    }
}
