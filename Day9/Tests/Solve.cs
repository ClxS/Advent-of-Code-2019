namespace Day9.Tests
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
        public void TestDataPart1()
        {
            using (new ScopedTimer(this.testOutputHelper))
            {
                Assert.Equal("109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99",
                    new Solver().Solve(new Data("109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99", 1)));
            }

            using (new ScopedTimer(this.testOutputHelper))
            {
                Assert.Equal("1125899906842624",
                    new Solver().Solve(new Data("104,1125899906842624,99", 1)));
            }
        }

        [Fact]
        public void InputDataPart1()
        {
            string value;

            var resourceName = "Day9.Tests.part1data.txt";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            using (new ScopedTimer(this.testOutputHelper))
            {
                var data = reader.ReadToEnd();
                value = new Solver().Solve(new Data(data, 1));
            }

            this.testOutputHelper.WriteLine($"BOOST Keycode: {value}");
        }

        [Fact]
        public void InputDataPart2()
        {
            string value;

            var resourceName = "Day9.Tests.part1data.txt";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            using (new ScopedTimer(this.testOutputHelper))
            {
                var data = reader.ReadToEnd();
                value = new Solver().Solve(new Data(data, 2));
            }

            this.testOutputHelper.WriteLine($"BOOST Keycode: {value}");
        }
    }
}
