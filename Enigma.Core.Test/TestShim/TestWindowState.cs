using System.Collections.Generic;
using Enigma.Core.Shim.Window;

namespace Enigma.Core.Test.TestShim;

public class TestWindowState : BaseWindowState
{
    /// <summary>
    /// Queue of Windows state.
    /// </summary>
    private readonly List<string> _windowStateQueue = new List<string>();

    /// <summary>
    /// Pushes a window state.
    /// </summary>
    /// <param name="state">Window state to push.</param>
    public void AddState(string state)
    {
        this._windowStateQueue.Add(state);
    }

    /// <summary>
    /// Returns the current active window title.
    /// </summary>
    /// <returns>The title of the active window, if one exists.</returns>
    public override string? GetActiveWindowTitle()
    {
        if (this._windowStateQueue.Count == 0) return null;
        var nextState = this._windowStateQueue[0];
        this._windowStateQueue.RemoveAt(0);
        return nextState;
    }
}