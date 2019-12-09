namespace Day7
{
    using System.Threading.Tasks;

    public interface ISolver
    {
        Task<int> Solve(Data inputData);
    }
}
