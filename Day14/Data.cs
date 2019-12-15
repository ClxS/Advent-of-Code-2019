namespace Day14
{
    public class Data
    {
        public Data(string reactionRecipes, long oreBudget = 0)
        {
            this.ReactionRecipes = reactionRecipes;
            this.OreBudget = oreBudget;
        }

        public string ReactionRecipes { get; }

        public long OreBudget { get; }
    }
}
