using System.Numerics;
using System.Threading;
using Enigma.Core.OpenVr.Model;
using Enigma.Core.Roblox;
using Enigma.Core.Test.TestShim;
using InputSimulatorStandard.Native;
using NUnit.Framework;

namespace Enigma.Core.Test.Roblox;

public class RobloxOutputTest
{
    public TestKeyboard TestKeyboard;
    public TestClipboard TestClipboard;
    public TestWindowState TestWindowState;
    public RobloxOutput RobloxOutput;

    [SetUp]
    public void SetUp()
    {
        TestKeyboard = new TestKeyboard();
        TestWindowState = new TestWindowState();
        TestClipboard = new TestClipboard();
        RobloxOutput = new RobloxOutput(this.TestKeyboard, this.TestClipboard, this.TestWindowState);
    }

    [Test]
    public void TestPushDataAsync()
    {
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        Assert.That(this.RobloxOutput.PushDataAsync("test").Result, Is.True);
        this.TestClipboard.AssertClipboard("test");
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.F13);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyDown, VirtualKeyCode.LCONTROL);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.VK_A);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.VK_V);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyUp, VirtualKeyCode.LCONTROL);
        this.TestKeyboard.AssertNoEvent();
    }

    [Test]
    public void TestPushDataAsyncNotFocused()
    {
        Assert.That(this.RobloxOutput.PushDataAsync("test").Result, Is.False);
        this.TestClipboard.AssertClipboard(null);
        this.TestKeyboard.AssertNoEvent();
    }

    [Test]
    public void TestPushDataAsyncFocusLostAfterClipboardSet()
    {
        this.TestWindowState.AddState("Roblox");
        Assert.That(this.RobloxOutput.PushDataAsync("test").Result, Is.False);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.F13);
        this.TestKeyboard.AssertNoEvent();
    }

    [Test]
    public void TestPushDataAsyncFocusLostBeforePast()
    {
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        Assert.That(this.RobloxOutput.PushDataAsync("test").Result, Is.False);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.F13);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyDown, VirtualKeyCode.LCONTROL);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.VK_A);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyUp, VirtualKeyCode.LCONTROL);
        this.TestKeyboard.AssertNoEvent();
    }
    
    [Test]
    public void TestPushDataAsyncNoRepeatHeartbeat()
    {
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        Assert.That(this.RobloxOutput.PushDataAsync("test").Result, Is.True);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.F13);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyDown, VirtualKeyCode.LCONTROL);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.VK_A);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.VK_V);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyUp, VirtualKeyCode.LCONTROL);
        this.TestKeyboard.AssertNoEvent();
        
        Assert.That(this.RobloxOutput.PushDataAsync("test").Result, Is.True);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyDown, VirtualKeyCode.LCONTROL);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.VK_A);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.VK_V);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyUp, VirtualKeyCode.LCONTROL);
        this.TestKeyboard.AssertNoEvent();
    }
    
    [Test]
    public void TestPushDataAsyncHeartbeat()
    {
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        Assert.That(this.RobloxOutput.PushDataAsync("test").Result, Is.True);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.F13);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyDown, VirtualKeyCode.LCONTROL);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.VK_A);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.VK_V);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyUp, VirtualKeyCode.LCONTROL);
        this.TestKeyboard.AssertNoEvent();
        Thread.Sleep(RobloxOutput.HeartbeatIntervalMilliseconds + 50);
        
        Assert.That(this.RobloxOutput.PushDataAsync("test").Result, Is.True);
        this.TestClipboard.AssertClipboard("test");
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.F13);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyDown, VirtualKeyCode.LCONTROL);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.VK_A);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.VK_V);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyUp, VirtualKeyCode.LCONTROL);
        this.TestKeyboard.AssertNoEvent();
    }
    
    [Test]
    public void TestPushTrackersAsync()
    {
        var trackerInputs = new TrackerInputList()
        {
            new TrackerInput()
            {
                Position = new Vector3(1, 2, 3),
                Rotation = new Quaternion(0.1f, 0.2f, 0.3f, 0.4f),
                Velocity = new Vector3(4, 5, 6),
            }
        };
        
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        this.TestWindowState.AddState("Roblox");
        Assert.That(this.RobloxOutput.PushTrackersAsync(trackerInputs).Result, Is.True);
        this.TestClipboard.AssertClipboard("1|1|1.000|2.000|3.000|0.100|0.200|0.300|0.400|4.000|5.000|6.000");
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.F13);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyDown, VirtualKeyCode.LCONTROL);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.VK_A);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyPress, VirtualKeyCode.VK_V);
        this.TestKeyboard.AssertEvent(TestKeyboard.KeyEvent.KeyUp, VirtualKeyCode.LCONTROL);
        this.TestKeyboard.AssertNoEvent();
    }
}