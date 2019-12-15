namespace Day15
{
    using System;
    using System.Collections.Generic;

    public class VisualiserUpdateEventArgs : EventArgs
    {
        public VisualiserUpdateEventArgs(IReadOnlyDictionary<(long X, long Y), ExploredTile> tiles)
        {
            this.Tiles = tiles;
        }

        public IReadOnlyDictionary<(long X, long Y), ExploredTile> Tiles { get; }
    }
}
