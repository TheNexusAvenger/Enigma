namespace Enigma.Core.Loop.Model;

public class LogSummary
{
    /// <summary>
    /// Number of completed ticks for outputting the OpenVR data to Roblox.
    /// This includes failed ticks, but those will have an error message.
    /// </summary>
    public long RobloxOutputTicksCompleted { get; set; } = 0;

    /// <summary>
    /// Number of skipped ticks for outputting the OpenVR data to Roblox.
    /// </summary>
    public long RobloxOutputTicksSkipped { get; set; } = 0;
    
    /// <summary>
    /// Average amount of time (in milliseconds) the Roblox output loop tick tick takes.
    /// If not provided, no attempts were ran or successful.
    /// </summary>
    public float? AverageRobloxOutputTimeMilliseconds { get; set; }
    
    /// <summary>
    /// Average amount of time (in milliseconds) the reading of OpenVR inputs takes.
    /// If not provided, no attempts were ran or successful.
    /// </summary>
    public float? AverageOpenVrReadTimeMilliseconds { get; set; }
    
    /// <summary>
    /// Average amount of time (in milliseconds) the data pushing to Roblox takes.
    /// If not provided, no attempts were ran or successful.
    /// </summary>
    public float? AverageTrackerDataPushTimeMilliseconds { get; set; }
}