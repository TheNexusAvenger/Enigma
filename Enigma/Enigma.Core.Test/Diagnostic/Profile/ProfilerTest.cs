using System.Threading.Tasks;
using Enigma.Core.Diagnostic.Profile;
using NUnit.Framework;

namespace Enigma.Core.Test.Diagnostic.Profile;

public class ProfilerTest
{
    [TearDown]
    public void Teardown()
    {
        Profiler.ResetAsync().Wait();
    }

    [Test]
    public void TestGetStatAsyncNoStat()
    {
        Assert.That(Profiler.GetStatAsync("UnknownStat").Result, Is.Null);
    }

    [Test]
    public void TestAddStatAsync()
    {
        Profiler.AddStatAsync("Test", 1).Wait();
        var originalStat = Profiler.GetStatAsync("Test").Result!;
        Assert.That(originalStat.Name, Is.EqualTo("Test"));
        Assert.That(originalStat.TotalEvents, Is.EqualTo(1));
        Assert.That(originalStat.AverageTime, Is.EqualTo(1));
        
        Profiler.AddStatAsync("Test", 3).Wait();
        Assert.That(originalStat.Name, Is.EqualTo("Test"));
        Assert.That(originalStat.TotalEvents, Is.EqualTo(1));
        Assert.That(originalStat.AverageTime, Is.EqualTo(1));
        var newStat = Profiler.GetStatAsync("Test").Result!;
        Assert.That(newStat.Name, Is.EqualTo("Test"));
        Assert.That(newStat.TotalEvents, Is.EqualTo(2));
        Assert.That(newStat.AverageTime, Is.EqualTo(2));
    }

    [Test]
    public void TestProfileAsync()
    {
        Profiler.ProfileAsync("Test", Task.Delay(50)).Wait();
        
        var stat = Profiler.GetStatAsync("Test").Result!;
        Assert.That(stat.Name, Is.EqualTo("Test"));
        Assert.That(stat.TotalEvents, Is.EqualTo(1));
        Assert.That(stat.AverageTime, Is.GreaterThan(25));
        Assert.That(stat.AverageTime, Is.LessThan(100));
    }

    [Test]
    public void TestProfileAsyncReturn()
    {
        var result = Profiler.ProfileAsync("Test", Task.Run(async () =>
        {
            await Task.Delay(50);
            return "TestResult";
        })).Result;
        
        Assert.That(result, Is.EqualTo("TestResult"));
        var stat = Profiler.GetStatAsync("Test").Result!;
        Assert.That(stat.Name, Is.EqualTo("Test"));
        Assert.That(stat.TotalEvents, Is.EqualTo(1));
        Assert.That(stat.AverageTime, Is.GreaterThan(25));
        Assert.That(stat.AverageTime, Is.LessThan(100));
    }

    [Test]
    public void TestProfileAsyncAction()
    {
        Profiler.ProfileAsync("Test", () =>
        {
            Task.Delay(50).Wait();
        }).Wait();
        
        var stat = Profiler.GetStatAsync("Test").Result!;
        Assert.That(stat.Name, Is.EqualTo("Test"));
        Assert.That(stat.TotalEvents, Is.EqualTo(1));
        Assert.That(stat.AverageTime, Is.GreaterThan(25));
        Assert.That(stat.AverageTime, Is.LessThan(100));
    }

    [Test]
    public void TestProfileAsyncFunction()
    {
        var result = Profiler.ProfileAsync("Test", () =>
        {
            Task.Delay(50).Wait();
            return "TestResult";
        }).Result;
        
        Assert.That(result, Is.EqualTo("TestResult"));
        var stat = Profiler.GetStatAsync("Test").Result!;
        Assert.That(stat.Name, Is.EqualTo("Test"));
        Assert.That(stat.TotalEvents, Is.EqualTo(1));
        Assert.That(stat.AverageTime, Is.GreaterThan(25));
        Assert.That(stat.AverageTime, Is.LessThan(100));
    }
}