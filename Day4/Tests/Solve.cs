namespace Day4.Tests
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class Timer : IDisposable
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly Stopwatch stopWatch;

        public Timer(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            this.stopWatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            this.testOutputHelper.WriteLine($"Elapsed: {this.stopWatch.ElapsedMilliseconds}ms");
        }
    }

    public class Solve
    {
        private readonly ITestOutputHelper testOutputHelper;

        public Solve(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Naive()
        {
            var solver = new NaiveSolver();
            int totalMatches;
            using (new Timer(this.testOutputHelper))
            {
                totalMatches = solver.Solve(new Data(206938, 679128));
            }

            this.testOutputHelper.WriteLine($"Total: {totalMatches}");
        }
    }
}
