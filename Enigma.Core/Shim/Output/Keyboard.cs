using InputSimulatorStandard;
using InputSimulatorStandard.Native;

namespace Enigma.Core.Shim.Output;

public class Keyboard : IKeyboard
{
    /// <summary>
    /// Keyboard simulator to send key inputs with.
    /// </summary>
    private readonly KeyboardSimulator _keyboardSimulator = new KeyboardSimulator();
    
    /// <summary>
    /// Sets a key as down.
    /// </summary>
    /// <param name="keyCode">Key to set as down.</param>
    public void KeyDown(VirtualKeyCode keyCode)
    {
        this._keyboardSimulator.KeyDown(keyCode);
    }

    /// <summary>
    /// Sets a key as up.
    /// </summary>
    /// <param name="keyCode">Key to set as up.</param>
    public void KeyUp(VirtualKeyCode keyCode)
    {
        this._keyboardSimulator.KeyUp(keyCode);
    }

    /// <summary>
    /// Presses a key (down, then up).
    /// </summary>
    /// <param name="keyCode">Key to press.</param>
    public void KeyPress(VirtualKeyCode keyCode)
    {
        this._keyboardSimulator.KeyPress(keyCode);
    }
}