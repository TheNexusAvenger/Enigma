using System;
using System.Threading.Tasks;
using Enigma.Core.Diagnostic;
using Enigma.Core.Diagnostic.Profile;
using Enigma.Core.Loop.Model;

namespace Enigma.Core.Loop;

public class LogOutputLoop : BaseLoop
{
    /// <summary>
    /// Percent of ticks to be skipped before the skipped ticks is a warn.
    /// Even with no logic running, loops commonly skip 1 tick.
    /// </summary>
    public const float SkippedTicksWarnPercent = 0.4f;

    /// <summary>
    /// Event for when a log entry is created.
    /// </summary>
    public event Action<LogSummary> LogEntryCreated;

    /// <summary>
    /// Roblox output loop to gather stats on.
    /// </summary>
    private readonly BaseLoop _robloxOutputLoop;
    
    /// <summary>
    /// Creates a log output loop.
    /// </summary>
    /// <param name="robloxOutputLoop">Roblox output loop to track.</param>
    public LogOutputLoop(BaseLoop robloxOutputLoop) : base(1000)
    {
        this._robloxOutputLoop = robloxOutputLoop;
        this.LogEntryCreated += this.OutputLog;
    }

    /// <summary>
    /// Logs a log entry.
    /// </summary>
    /// <param name="logEntry">Log entry data to log.</param>
    private void OutputLog(LogSummary logEntry)
    {
        // Log the ticks.
        if (logEntry.RobloxOutputTicksDataSent == 0 && logEntry.RobloxOutputTicksSkipped == 0) return;
        Logger.Debug($"Roblox client data send tick rate: {logEntry.RobloxOutputTicksCompleted} completed, {logEntry.RobloxOutputTicksSkipped} skipped");
        if (logEntry.RobloxOutputTicksSkipped > 0)
        {
            var skippedMessage = $"Skipped {logEntry.RobloxOutputTicksSkipped} ticks when sending data. This might happen when a previous data send took too long.";
            if (((float) logEntry.RobloxOutputTicksSkipped / (logEntry.RobloxOutputTicksCompleted + logEntry.RobloxOutputTicksSkipped)) >= SkippedTicksWarnPercent)
            {
                Logger.Warn(skippedMessage);
            }
            else
            {
                Logger.Debug(skippedMessage);
            }
        }
        
        // Log the time.
        if (logEntry.AverageRobloxOutputTimeMilliseconds.HasValue)
        {
            Logger.Trace($"Average tick duration: {logEntry.AverageRobloxOutputTimeMilliseconds:f3} ms");
        }
        if (logEntry.AverageOpenVrReadTimeMilliseconds.HasValue)
        {
            Logger.Trace($"Average OpenVR input read time: {logEntry.AverageOpenVrReadTimeMilliseconds:f3} ms");
        }
        if (logEntry.AverageTrackerDataPushTimeMilliseconds.HasValue)
        {
            Logger.Trace($"Roblox data send tick duration: {logEntry.AverageTrackerDataPushTimeMilliseconds:f3} ms");
        }
    }
    
    /// <summary>
    /// Runs a step in the loop.
    /// </summary>
    public override async Task StepAsync()
    {
        // Create the log summary and invoke the event with the summary.
        var logSummary = new LogSummary()
        {
            RobloxOutputTicksCompleted = this._robloxOutputLoop.TicksCompleted,
            RobloxOutputTicksSkipped = this._robloxOutputLoop.TicksSkipped,
            RobloxOutputTicksDataSent = (await Profiler.GetStatAsync("PushTrackerDataSentTotal"))?.TotalEvents ?? 0,
            AverageRobloxOutputTimeMilliseconds = (await Profiler.GetStatAsync(this._robloxOutputLoop.ProfilerStatName))?.AverageTime,
            AverageOpenVrReadTimeMilliseconds = (await Profiler.GetStatAsync("OpenVRGetInputs"))?.AverageTime,
            AverageTrackerDataPushTimeMilliseconds =(await Profiler.GetStatAsync("PushTrackerData"))?.AverageTime,
        };
        this.LogEntryCreated?.Invoke(logSummary);
        
        // Clear the stats.
        await this._robloxOutputLoop.ResetStatsAsync();
        await Profiler.ResetAsync();
    }
}