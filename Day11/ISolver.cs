namespace Day11
{
    public interface ISolver
    {
        (int Count, char[,] Pattern) Solve(Data inputData);
    }
}
