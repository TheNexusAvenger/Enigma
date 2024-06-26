using System.Collections.Generic;
using System.Linq;
using Enigma.Core.Shim.Output;
using InputSimulatorStandard.Native;
using NUnit.Framework;

namespace Enigma.Core.Test.TestShim;

public class TestKeyboard : IKeyboard
{
    public enum KeyEvent
    {
        KeyDown,
        KeyUp,
        KeyPress,
    }

    /// <summary>
    /// Keyboard events that have occured.
    /// </summary>
    private readonly List<(KeyEvent, VirtualKeyCode)> _events = new List<(KeyEvent, VirtualKeyCode)>();
    
    /// <summary>
    /// Sets a key as down.
    /// </summary>
    /// <param name="keyCode">Key to set as down.</param>
    public void KeyDown(VirtualKeyCode keyCode)
    {
        this._events.Add((KeyEvent.KeyDown, keyCode));
    }

    /// <summary>
    /// Sets a key as up.
    /// </summary>
    /// <param name="keyCode">Key to set as up.</param>
    public void KeyUp(VirtualKeyCode keyCode)
    {
        this._events.Add((KeyEvent.KeyUp, keyCode));
    }

    /// <summary>
    /// Presses a key (down, then up).
    /// </summary>
    /// <param name="keyCode">Key to press.</param>
    public void KeyPress(VirtualKeyCode keyCode)
    {
        this._events.Add((KeyEvent.KeyPress, keyCode));
    }
    
    /// <summary>
    /// Asserts the next key event and code.
    /// </summary>
    /// <param name="keyEvent">Key event to assert.</param>
    /// <param name="keyCode">Key code to assert.</param>
    public void AssertEvent(KeyEvent keyEvent, VirtualKeyCode keyCode)
    {
        var nextEvent = this._events.FirstOrDefault();
        if (nextEvent == default) throw new AssertionException("No remaining key events found.");
        this._events.RemoveAt(0);
        Assert.Multiple(() =>
        {
            Assert.That(nextEvent.Item1, Is.EqualTo(keyEvent));
            Assert.That(nextEvent.Item2, Is.EqualTo(keyCode));
        });
    }

    /// <summary>
    /// Asserts there are no more remaining events.
    /// </summary>
    public void AssertNoEvent()
    {
        Assert.That(this._events.Count(), Is.EqualTo(0));
    }
}