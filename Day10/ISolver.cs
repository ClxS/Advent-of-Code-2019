namespace Day10
{
    public interface ISolver
    {
        (int X , int Y, int VisibleCount) Solve(Data inputData);
    }
}
