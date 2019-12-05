namespace Day2.Tests
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
        public void ExampleData()
        {
            using (new ScopedTimer(this.testOutputHelper))
            {
                var solver = new Solver();
                Assert.Equal(3500, solver.Solve(new Data("1,9,10,3,2,3,11,0,99,30,40,50")));
                Assert.Equal(2, solver.Solve(new Data("1,0,0,0,99")));
                Assert.Equal(2, solver.Solve(new Data("2,3,0,3,99")));
                Assert.Equal(2, solver.Solve(new Data("2,4,4,5,99,0")));
                Assert.Equal(30, solver.Solve(new Data("1,1,1,4,99,5,6,0,99")));
            }
        }

        [Fact]
        public void InputData()
        {
            int value;
            using (new ScopedTimer(this.testOutputHelper))
            {
                var solver = new Solver();
                value = solver.Solve(new Data(
                    "1,12,2,3,1,1,2,3,1,3,4,3,1,5,0,3,2,1,10,19,1,6,19,23,2,23,6,27,2,6,27,31,2," +
                    "13,31,35,1,10,35,39,2,39,13,43,1,43,13,47,1,6,47,51,1,10,51,55,2,55,6,59,1," +
                    "5,59,63,2,9,63,67,1,6,67,71,2,9,71,75,1,6,75,79,2,79,13,83,1,83,10,87,1,13," +
                    "87,91,1,91,10,95,2,9,95,99,1,5,99,103,2,10,103,107,1,107,2,111,1,111,5,0,99," +
                    "2,14,0,0"));
            }

            this.testOutputHelper.WriteLine($"Pos 0 Value: {value}");
        }

        [Fact]
        public void BruteForcePart2Data()
        {
            int result = 0;
            using (new ScopedTimer(this.testOutputHelper))
            {
                var solver = new Solver();
                for (var noun = 0; noun < 100; noun++)
                {
                    for (var verb = 0; verb < 100; verb++)
                    {
                        var value = solver.Solve(new Data(
                            $"1,{noun},{verb},3,1,1,2,3,1,3,4,3,1,5,0,3,2,1,10,19,1,6,19,23,2,23,6,27,2,6,27,31,2," +
                            "13,31,35,1,10,35,39,2,39,13,43,1,43,13,47,1,6,47,51,1,10,51,55,2,55,6,59,1," +
                            "5,59,63,2,9,63,67,1,6,67,71,2,9,71,75,1,6,75,79,2,79,13,83,1,83,10,87,1,13," +
                            "87,91,1,91,10,95,2,9,95,99,1,5,99,103,2,10,103,107,1,107,2,111,1,111,5,0,99," +
                            "2,14,0,0"));

                        if (value == 19690720)
                        {
                            result = 100 * noun + verb;
                        }
                    }
                }
            }

            this.testOutputHelper.WriteLine($"Pos 0 Value: {result}");
        }
    }
}
