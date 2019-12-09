namespace Day7
{
    public class Data
    {
        public Data(string opcodes, int minimumPhaseSettings, int maximumPhaseSettings, bool useLoopback)
        {
            this.OpCodes = opcodes;
            this.MinimumPhaseSettings = minimumPhaseSettings;
            this.MaximumPhaseSettings = maximumPhaseSettings;
            this.UseLoopback = useLoopback;
        }

        public string OpCodes { get; }

        public int MinimumPhaseSettings { get; }

        public int MaximumPhaseSettings { get; }

        public bool UseLoopback { get; }
    }
}
