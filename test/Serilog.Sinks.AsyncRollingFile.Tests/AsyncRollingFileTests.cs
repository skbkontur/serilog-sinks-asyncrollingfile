using System;
using Xunit;

namespace Serilog.Sinks.ElkStreams.Tests
{
    public class AsyncRollingFileTests
    {
        [Fact]
        public void Send()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.AsyncRollingFile("{Date}.log")
                .CreateLogger();

            Log.Logger.Debug("Hello, {World}!", "world");
            Log.Logger.Debug(new Exception(), "Hello, {World}!", "world");
            Log.Logger.Information("Hello, {World}!", "world");
            Log.Logger.Information(new Exception(), "Hello, {World}!", "world");
            Log.Logger.Warning("Hello, {World}!", "world");
            Log.Logger.Warning(new Exception(), "Hello, {World}!", "world");
            Log.Logger.Error("Hello, {World}!", "world");
            Log.Logger.Error(new Exception(), "Hello, {World}!", "world");
            Log.Logger.Fatal("Hello, {World}!", "world");
            Log.Logger.Fatal(new Exception(), "Hello, {World}!", "world");

            Log.CloseAndFlush();
        }
    }
}
