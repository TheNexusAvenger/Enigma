﻿using System.Linq;
using Enigma.Core.Roblox;

namespace Enigma.Core.Shim.Window;

public abstract class BaseWindowState
{
    /// <summary>
    /// Roblox Studio state to check for.
    /// </summary>
    private readonly RobloxStudioState _robloxStudioState;

    /// <summary>
    /// Creates a base window state.
    /// </summary>
    /// <param name="robloxStudioState">Roblox Studio state to use.</param>
    public BaseWindowState(RobloxStudioState robloxStudioState)
    {
        this._robloxStudioState = robloxStudioState;
    }
    
    /// <summary>
    /// Returns if Roblox is focused.
    /// </summary>
    /// <returns>Whether Roblox is focused or not.</returns>
    public bool IsRobloxFocused()
    {
        // Return false if no window is selected or doesn't contain Roblox.
        var focusedWindowName = GetActiveWindowTitle();
        if (focusedWindowName == null) return false;
        if (!focusedWindowName.Contains("Roblox")) return false;
        
        // Return false for Roblox Studio with 2 dashes or the companion plugin is active.
        // Currently, this means a script is open.
        if (focusedWindowName.EndsWith("Roblox Studio"))
        {
            if (this._robloxStudioState.IsRobloxStudioConnected()) return false;
            return focusedWindowName.ToCharArray().Count(character => character == '-') <= 1;
        }
        
        // Return true if the windows is Roblox.
        // This attempts to avoid other windows that contain Roblox.
        return focusedWindowName == "Roblox";
    }

    /// <summary>
    /// Returns the current active window title.
    /// </summary>
    /// <returns>The title of the active window, if one exists.</returns>
    public abstract string? GetActiveWindowTitle();
}