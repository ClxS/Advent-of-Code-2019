namespace Day7
{
    public class Data
    {
        public Data(string opcodes, int minimumPhaseSettings, int maximumPhaseSettings)
        {
            this.OpCodes = opcodes;
            this.MinimumPhaseSettings = minimumPhaseSettings;
            this.MaximumPhaseSettings = maximumPhaseSettings;
        }

        public string OpCodes { get; }

        public int MinimumPhaseSettings { get; }

        public int MaximumPhaseSettings { get; }
    }
}
