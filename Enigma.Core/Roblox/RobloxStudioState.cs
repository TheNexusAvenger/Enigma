using System.Diagnostics;

namespace Enigma.Core.Roblox;

public class RobloxStudioState
{
    /// <summary>
    /// Timeout (in milliseconds) for Roblox Studio to send a heartbeat.
    /// </summary>
    public long RobloxStudioHeartbeatTimeoutMilliseconds { get; set; } = 1000;
    
    /// <summary>
    /// Stopwatch used to track when the last heartbeat was sent from Roblox Studio.
    /// </summary>
    private readonly Stopwatch _stopwatch = new Stopwatch();

    /// <summary>
    /// Returns if the Roblox Studio companion plugin responded recently enough to not use the TextBox capture method.
    /// </summary>
    /// <returns>Whether Roblox Studio's plugin is considered connected.</returns>
    public bool IsRobloxStudioConnected()
    {
        return this._stopwatch.IsRunning && this._stopwatch.ElapsedMilliseconds <= this.RobloxStudioHeartbeatTimeoutMilliseconds;
    }

    /// <summary>
    /// Invoked when Roblox Studio sends a heartbeat.
    /// </summary>
    public void HeartbeatSent()
    {
        this._stopwatch.Restart();
    }
}