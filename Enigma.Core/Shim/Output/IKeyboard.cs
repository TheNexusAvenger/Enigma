using InputSimulatorStandard.Native;

namespace Enigma.Core.Shim.Output;

public interface IKeyboard
{
    /// <summary>
    /// Sets a key as down.
    /// </summary>
    /// <param name="keyCode">Key to set as down.</param>
    void KeyDown(VirtualKeyCode keyCode);
    
    /// <summary>
    /// Sets a key as up.
    /// </summary>
    /// <param name="keyCode">Key to set as up.</param>
    void KeyUp(VirtualKeyCode keyCode);
    
    /// <summary>
    /// Presses a key (down, then up).
    /// </summary>
    /// <param name="keyCode">Key to press.</param>
    void KeyPress(VirtualKeyCode keyCode);
}