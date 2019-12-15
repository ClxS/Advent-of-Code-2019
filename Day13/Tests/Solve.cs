namespace Day13.Tests
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
            var resourceName = "Day13.Tests.part1data.txt";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            (int BlocksRemaining, long Score) value;
            using (new ScopedTimer(this.testOutputHelper))
            {
                var data = reader.ReadToEnd();
                value = new Solver().Solve(new Data(data, false));
            }

            this.testOutputHelper.WriteLine($"Blocks Remaining: {value.BlocksRemaining}, Score: {value.Score}");
        }

        [Fact]
        public void InputDataPart2()
        {
            var resourceName = "Day13.Tests.part1data.txt";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            (int BlocksRemaining, long Score) value;
            using (new ScopedTimer(this.testOutputHelper))
            {
                var data = reader.ReadToEnd();
                data = '2' + data.Substring(1);
                value = new Solver().Solve(new Data(data, false));
            }

            this.testOutputHelper.WriteLine($"Blocks Remaining: {value.BlocksRemaining}, Score: {value.Score}");
        }
    }
}
