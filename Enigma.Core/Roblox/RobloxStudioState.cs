using System.Diagnostics;
using System.Threading.Tasks;
using Enigma.Core.Diagnostic;

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
    /// Creates a Roblox Studio state.
    /// </summary>
    public RobloxStudioState()
    {
        // Occasionally check if Roblox Studio is connected to update the logging.
        Task.Run(async () =>
        {
            while (true)
            {
                this.IsRobloxStudioConnected();
                await Task.Delay(100);
            }
        });
    }

    /// <summary>
    /// Returns if the Roblox Studio companion plugin responded recently enough to not use the TextBox capture method.
    /// </summary>
    /// <returns>Whether Roblox Studio's plugin is considered connected.</returns>
    public bool IsRobloxStudioConnected()
    {
        if (this._stopwatch.IsRunning && this._stopwatch.ElapsedMilliseconds > this.RobloxStudioHeartbeatTimeoutMilliseconds)
        {
            this._stopwatch.Stop();
            Logger.Info("Roblox Studio companion app disconnected.");
        }
        return this._stopwatch.IsRunning;
    }

    /// <summary>
    /// Invoked when Roblox Studio sends a heartbeat.
    /// </summary>
    public void HeartbeatSent()
    {
        if (!this._stopwatch.IsRunning)
        {
            Logger.Info("Roblox Studio companion app connected.");
        }
        this._stopwatch.Restart();
    }
}