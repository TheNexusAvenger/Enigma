using System;
using System.Runtime.InteropServices;
using System.Text;
using Enigma.Core.Roblox;

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
    /// Creates a Windows-specific base window state.
    /// </summary>
    /// <param name="robloxStudioState">Roblox Studio state to use.</param>
    public WindowsWindowState(RobloxStudioState robloxStudioState) : base(robloxStudioState)
    {
        
    }
    
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