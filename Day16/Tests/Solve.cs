namespace Day16.Tests
{
    using System;
    using System.IO;
    using System.Linq;
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
        public void TestPart1()
        {
            using (new ScopedTimer(this.testOutputHelper))
            {
                Assert.Equal("48226158", new Solver().Solve(new Data(new []{ '1', '2', '3', '4', '5', '6', '7', '8' }, 1, false)));
            }
        }

        [Fact]
        public void InputDataPart1()
        {
            var resourceName = "Day16.Tests.inputdata.txt";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            string value;
            using (new ScopedTimer(this.testOutputHelper))
            {
                var data = reader.ReadToEnd().ToCharArray();
                value = new Solver().Solve(new Data(data, 100, false));
            }

            this.testOutputHelper.WriteLine($"Final Value: {value}");
        }

        [Fact]
        public void InputDataPart2()
        {
            var resourceName = "Day16.Tests.inputdata.txt";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            string value;
            using (new ScopedTimer(this.testOutputHelper))
            {
                var data = reader.ReadToEnd().ToCharArray();
                var trueData = new char[10000 * data.Length];
                for (var i = 0; i < 10000; i++)
                {
                    Buffer.BlockCopy(data, 0, trueData, i * data.Length, data.Length);
                }

                value = new Solver().Solve(new Data(trueData, 100, true));
            }

            this.testOutputHelper.WriteLine($"Final Value: {value}");
        }
    }
}
