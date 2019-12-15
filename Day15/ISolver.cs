namespace Day15
{
    using System;

    public interface ISolver
    {
        event EventHandler<VisualiserUpdateEventArgs> VisualisableUpdate;

        int Solve(Data inputData);
    }
}
