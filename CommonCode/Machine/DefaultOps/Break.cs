namespace CommonCode.Machine.DefaultOps
{
    using System;

    public class Break : IOp
    {
        public int DataLength => 0;

        public bool Act(ReadOnlySpan<int> opData, Memory<int> memory)
        {
            return false;
        }
    }
}
