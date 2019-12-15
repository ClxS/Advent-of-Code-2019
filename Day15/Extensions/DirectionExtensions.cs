namespace Day15.Extensions
{
    using System;

    public static class DirectionExtensions
    {
        public static Direction Reverse(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North: return Direction.South;
                case Direction.South: return Direction.North;
                case Direction.West: return Direction.East;
                case Direction.East: return Direction.West;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}
