namespace Day15.Tests
{
    using System.IO;
    using System.Reflection;
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
        public void InputDataPart1()
        {
            var resourceName = "Day15.Tests.inputdata.txt";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            int value;
            using (new ScopedTimer(this.testOutputHelper))
            {
                var data = reader.ReadToEnd();
                value = new Solver().Solve(new Data(data, false, new []{ DroidState.Explore, DroidState.CalculateDistance }));
            }

            this.testOutputHelper.WriteLine($"Fewest Movements Required: {value}");
        }

        [Fact]
        public void InputDataPart2()
        {
            var resourceName = "Day15.Tests.inputdata.txt";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            int value;
            using (new ScopedTimer(this.testOutputHelper))
            {
                var data = reader.ReadToEnd();
                value = new Solver().Solve(new Data(data, false, new[] { DroidState.Explore, DroidState.CalculateOxygenDistributionTime }));
            }

            this.testOutputHelper.WriteLine($"Disbursement Time: {value}");
        }
    }
}
