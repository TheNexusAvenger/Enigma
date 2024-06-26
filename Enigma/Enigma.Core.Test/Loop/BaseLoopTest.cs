using System;
using System.Threading;
using System.Threading.Tasks;
using Enigma.Core.Diagnostic.Profile;
using Enigma.Core.Loop;
using NUnit.Framework;

namespace Enigma.Core.Test.Loop;

public class BaseLoopTest
{
    private class TestableLoop : BaseLoop
    {
        private readonly int _delayMilliseconds;
        
        public TestableLoop(int delayMilliseconds) : base(15)
        {
            this._delayMilliseconds = delayMilliseconds;
        }

        public override async Task StepAsync()
        {
            if (this._delayMilliseconds == 0) return;
            await Task.Delay(this._delayMilliseconds);
        }
    }
    
    private class TestableExceptionLoop : BaseLoop
    {
        public TestableExceptionLoop() : base(15)
        {
            
        }

        public override Task StepAsync()
        {
            throw new Exception("Test exception");
        }
    }
    
    [TearDown]
    public void Teardown()
    {
        Profiler.ResetAsync().Wait();
    }

    [Test]
    public void TestTickAsync()
    {
        var loop = new TestableLoop(0);
        loop.TickAsync().Wait();
        Assert.That(loop.TicksCompleted, Is.EqualTo(1));
        Assert.That(loop.TicksSkipped, Is.EqualTo(0));
        var stat = Profiler.GetStatAsync("TestableLoop_TickDuration").Result!;
        Assert.That(stat.Name, Is.EqualTo("TestableLoop_TickDuration"));
        Assert.That(stat.TotalEvents, Is.EqualTo(1));
        Assert.That(stat.AverageTime, Is.EqualTo(0));
    }

    [Test]
    public void TestTickAsyncMultiple()
    {
        var loop = new TestableLoop(0);
        loop.TickAsync().Wait();
        loop.TickAsync().Wait();
        loop.TickAsync().Wait();
        Assert.That(loop.TicksCompleted, Is.EqualTo(3));
        Assert.That(loop.TicksSkipped, Is.EqualTo(0));
        var stat = Profiler.GetStatAsync("TestableLoop_TickDuration").Result!;
        Assert.That(stat.Name, Is.EqualTo("TestableLoop_TickDuration"));
        Assert.That(stat.TotalEvents, Is.EqualTo(3));
        Assert.That(stat.AverageTime, Is.EqualTo(0));
    }

    [Test]
    public void TestTickAsyncSkipMultiple()
    {
        var loop = new TestableLoop(50);
        var originalAwaitble = loop.TickAsync();
        loop.TickAsync().Wait();
        originalAwaitble.Wait();
        Assert.That(loop.TicksCompleted, Is.EqualTo(1));
        Assert.That(loop.TicksSkipped, Is.EqualTo(1));
        var stat = Profiler.GetStatAsync("TestableLoop_TickDuration").Result!;
        Assert.That(stat.Name, Is.EqualTo("TestableLoop_TickDuration"));
        Assert.That(stat.TotalEvents, Is.EqualTo(1));
        Assert.That(stat.AverageTime, Is.GreaterThan(25));
        Assert.That(stat.AverageTime, Is.LessThan(100));
    }

    [Test]
    public void TestTickAsyncException()
    {
        var loop = new TestableExceptionLoop();
        loop.TickAsync().Wait();
        Assert.That(loop.TicksCompleted, Is.EqualTo(1));
        Assert.That(loop.TicksSkipped, Is.EqualTo(0));
        Assert.That(Profiler.GetStatAsync("TestableLoop_TickDuration").Result, Is.Null);
    }

    [Test]
    public void TestStart()
    {
        var loop = new TestableLoop(0);
        loop.Start();
        Thread.Sleep(60);
        loop.Stop();
        
        var ticksCompleted = loop.TicksCompleted;
        Assert.That(ticksCompleted, Is.GreaterThan(1));
        Assert.That(ticksCompleted, Is.LessThan(5));
        Assert.That(loop.TicksSkipped, Is.EqualTo(0));
        var stat = Profiler.GetStatAsync("TestableLoop_TickDuration").Result!;
        Assert.That(stat.Name, Is.EqualTo("TestableLoop_TickDuration"));
        Assert.That(stat.TotalEvents, Is.EqualTo(loop.TicksCompleted));
        Assert.That(stat.AverageTime, Is.EqualTo(0));
        
        Thread.Sleep(30);
        Assert.That(loop.TicksCompleted, Is.EqualTo(ticksCompleted));
        Assert.That(loop.TicksSkipped, Is.EqualTo(0));
    }
}