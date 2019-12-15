namespace Day13
{
    public interface ISolver
    {
        (int BlocksRemaining, long Score) Solve(Data inputData);
    }
}
