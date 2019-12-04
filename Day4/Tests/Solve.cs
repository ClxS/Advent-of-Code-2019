namespace Day4.Tests
{
    using CommonCode;
    using Xunit;
    using Xunit.Abstractions;

    public class Solve
    {
        private readonly ITestOutputHelper testOutputHelper;

        public Solve(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void NaivePart1()
        {
            var solver = new NaiveSolver(false);
            int totalMatches;
            using (new ScopedTimer(this.testOutputHelper))
            {
                totalMatches = solver.Solve(new Data(206938, 679128));
            }

            this.testOutputHelper.WriteLine($"Total: {totalMatches}");
        }

        [Fact]
        public void NaivePart2()
        {
            var solver = new NaiveSolver(true);
            int totalMatches;
            using (new ScopedTimer(this.testOutputHelper))
            {
                totalMatches = solver.Solve(new Data(206938, 679128));
            }

            this.testOutputHelper.WriteLine($"Total: {totalMatches}");
        }
    }
}
