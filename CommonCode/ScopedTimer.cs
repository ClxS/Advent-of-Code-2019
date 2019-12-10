namespace CommonCode
{
    using System;
    using System.Diagnostics;
    using Xunit.Abstractions;

    public class ScopedTimer : IDisposable
    {
        private readonly Stopwatch stopWatch;
        private readonly ITestOutputHelper testOutputHelper;

        public ScopedTimer(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            this.stopWatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            this.stopWatch.Stop();
            this.testOutputHelper.WriteLine($"Elapsed: {this.stopWatch.ElapsedMilliseconds}ms");
        }
    }
}
