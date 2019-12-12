namespace Day11.Tests
{
    using System.IO;
    using System.Reflection;
    using System.Text;
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
            var resourceName = "Day11.Tests.part1data.txt";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            (int Count, char[,] Pattern) value;
            using (new ScopedTimer(this.testOutputHelper))
            {
                var data = reader.ReadToEnd();
                value = new Solver().Solve(new Data(data, 0, false));
            }

            this.testOutputHelper.WriteLine($"Painted: {value.Count}");
        }

        [Fact]
        public void InputDataPart2()
        {
            var resourceName = "Day11.Tests.part1data.txt";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            (int Count, char[,] Pattern) value;
            using (new ScopedTimer(this.testOutputHelper))
            {
                var data = reader.ReadToEnd();
                value = new Solver().Solve(new Data(data, 1, true));
            }

            var sb = new StringBuilder();
            for (var y = 0; y < value.Pattern.GetLength(1); y++)
            {
                for (var x = 0; x < value.Pattern.GetLength(0); x++)
                {
                    sb.Append(value.Pattern[x, y]);
                }

                sb.Append('\n');
            }

            this.testOutputHelper.WriteLine($"Map:\n{sb}");

        }
    }
}
