namespace CommonCode.Machine.DefaultOps
{
    using System.Threading.Tasks;

    public interface IAsyncOp : IOp
    {
        Task Act(IntMachine machine, long[] opData, byte[] modes);
    }
}
