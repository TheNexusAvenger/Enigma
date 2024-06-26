using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Enigma.Core.Shim.Window;

public class WindowsWindowState : BaseWindowState
{
    /// <summary>
    /// Returns the current foreground window.
    /// </summary>
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    /// <summary>
    /// Returns the current text of a window.
    /// </summary>
    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
    
    /// <summary>
    /// Returns the current active window title.
    /// </summary>
    /// <returns>The title of the active window, if one exists.</returns>
    public override string? GetActiveWindowTitle()
    {
        const int maxCharacters = 256;
        var windowTextBuffer = new StringBuilder(maxCharacters);
        var windowHandle = GetForegroundWindow();
        return GetWindowText(windowHandle, windowTextBuffer, maxCharacters) > 0 ? windowTextBuffer.ToString() : null;
    }
}