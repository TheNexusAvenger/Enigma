using System.Threading;
using Enigma.Core.Roblox;
using NUnit.Framework;

namespace Enigma.Core.Test.Roblox;

public class RobloxStudioStateTest
{
    private RobloxStudioState _testRobloxStudioState;

    [SetUp]
    public void SetUp()
    {
        _testRobloxStudioState = new RobloxStudioState()
        {
            RobloxStudioHeartbeatTimeoutMilliseconds = 30,
        };
    }

    [Test]
    public void TestIsRobloxStudioConnectedDefault()
    {
        Assert.That(_testRobloxStudioState.IsRobloxStudioConnected(), Is.False);
    }

    [Test]
    public void TestIsRobloxStudioConnectedHeartbeat()
    {
        _testRobloxStudioState.HeartbeatSent();
        Assert.That(_testRobloxStudioState.IsRobloxStudioConnected(), Is.True);
    }

    [Test]
    public void TestIsRobloxStudioConnectedExpiredHeartbeat()
    {
        _testRobloxStudioState.HeartbeatSent();
        Thread.Sleep(50);
        Assert.That(_testRobloxStudioState.IsRobloxStudioConnected(), Is.False);
    }
}