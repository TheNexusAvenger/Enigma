using Enigma.Core.Diagnostic.Profile;
using Enigma.Core.Loop;
using Enigma.Core.Loop.Model;
using NUnit.Framework;

namespace Enigma.Core.Test.Loop;

public class LogOutputLoopTest
{
    private BaseLoopTest.TestableLoop _testableLoop;
    private LogOutputLoop _logOutputLoop;
    
    [SetUp]
    public void SetUp()
    {
        _testableLoop = new BaseLoopTest.TestableLoop(50);
        _logOutputLoop = new LogOutputLoop(_testableLoop);
    }
    
    [TearDown]
    public void Teardown()
    {
        Profiler.ResetAsync().Wait();
    }
    
    [Test]
    public void TestStepAsync()
    {
        var originalAwaitable = _testableLoop.TickAsync();
        _testableLoop.TickAsync().Wait();
        originalAwaitable.Wait();
        
        LogSummary? logSummary = default;
        _logOutputLoop.LogEntryCreated += (newLogSummary) =>
        {
            logSummary = newLogSummary;
        };
        Profiler.AddStatAsync("OpenVRGetInputs", 2).Wait();
        Profiler.AddStatAsync("PushTrackerData", 1).Wait();
        Profiler.AddStatAsync("PushTrackerDataSentTotal").Wait();
        _logOutputLoop.StepAsync().Wait();
        
        Assert.That(logSummary!.RobloxOutputTicksCompleted, Is.EqualTo(1));
        Assert.That(logSummary!.RobloxOutputTicksSkipped, Is.EqualTo(1));
        Assert.That(logSummary!.RobloxOutputTicksDataSent, Is.EqualTo(1));
        Assert.That(logSummary!.AverageRobloxOutputTimeMilliseconds, Is.GreaterThan(25));
        Assert.That(logSummary!.AverageRobloxOutputTimeMilliseconds, Is.LessThan(100));
        Assert.That(logSummary!.AverageOpenVrReadTimeMilliseconds, Is.EqualTo(2));
        Assert.That(logSummary!.AverageTrackerDataPushTimeMilliseconds, Is.EqualTo(1));
        
        _logOutputLoop.StepAsync().Wait();
        Assert.That(logSummary!.RobloxOutputTicksCompleted, Is.EqualTo(0));
        Assert.That(logSummary!.RobloxOutputTicksSkipped, Is.EqualTo(0));
        Assert.That(logSummary!.RobloxOutputTicksDataSent, Is.EqualTo(0));
        Assert.That(logSummary!.AverageRobloxOutputTimeMilliseconds, Is.Null);
        Assert.That(logSummary!.AverageOpenVrReadTimeMilliseconds, Is.Null);
        Assert.That(logSummary!.AverageTrackerDataPushTimeMilliseconds, Is.Null);
    }
}