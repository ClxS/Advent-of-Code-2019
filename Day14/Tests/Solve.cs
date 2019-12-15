namespace Day14.Tests
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
        public void Part1TestData()
        {
            var resourceName = "Day14.Tests.testdata1.txt";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            using (new ScopedTimer(this.testOutputHelper))
            {
                var data = reader.ReadToEnd();
                Assert.Equal(31, new Solver().Solve(new Data(data)));
            }
        }

        [Fact]
        public void InputDataPart1()
        {
            var resourceName = "Day14.Tests.inputdata.txt";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            long oreRequired;
            using (new ScopedTimer(this.testOutputHelper))
            {
                var data = reader.ReadToEnd();
                oreRequired = new Solver().Solve(new Data(data));
            }

            this.testOutputHelper.WriteLine($"Ore Required: {oreRequired}");
        }

        [Fact]
        public void InputDataPart2()
        {
            var resourceName = "Day14.Tests.inputdata.txt";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            long oreRequired;
            using (new ScopedTimer(this.testOutputHelper))
            {
                var data = reader.ReadToEnd();
                oreRequired = new Solver().Solve(new Data(data, 1_000_000_000_000));
            }

            this.testOutputHelper.WriteLine($"Fuel Produced: {oreRequired}");
        }
    }
}
