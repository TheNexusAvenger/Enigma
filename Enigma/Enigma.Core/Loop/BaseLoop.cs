using System;
using System.Threading;
using System.Threading.Tasks;
using Enigma.Core.Diagnostic.Profile;

namespace Enigma.Core.Loop;

public abstract class BaseLoop
{
    /// <summary>
    /// Semaphore used for updating stats.
    /// </summary>
    private readonly SemaphoreSlim _statsSemaphore = new SemaphoreSlim(1);
    
    /// <summary>
    /// Timer used to perform events.
    /// </summary>
    private readonly System.Timers.Timer _timer;
    
    /// <summary>
    /// Total number of ticks that were completed.
    /// </summary>
    public long TicksCompleted { get; private set; }
    
    /// <summary>
    /// Total number of ticks that were skipped.
    /// </summary>
    public long TicksSkipped { get; private set; }

    /// <summary>
    /// Name of the profiler stat name.
    /// </summary>
    public string ProfilerStatName => $"{this.GetType().Name}_TickDuration";

    /// <summary>
    /// Whether a tick is running.
    /// </summary>
    private bool _tickRunning = false;
    
    /// <summary>
    /// Creates a base loop.
    /// </summary>
    /// <param name="interval">Interval in milliseconds. Note that less than 15ms doesn't work on Windows.</param>
    public BaseLoop(double interval)
    {
        this._timer = new System.Timers.Timer()
        {
            Interval = interval,
            AutoReset = true,
        };
        this._timer.Elapsed += (_, _) => this.TickAsync().Wait();
    }

    /// <summary>
    /// Performs a step in the loop and tracks stats.
    /// </summary>
    public async Task TickAsync()
    {
        // Return if another tick is running.
        await this._statsSemaphore.WaitAsync();
        if (this._tickRunning)
        {
            this.TicksSkipped += 1;
            this._statsSemaphore.Release();
            return;
        }
        this._tickRunning = true;
        this._statsSemaphore.Release();
        
        // Perform the tick.
        try
        {
            await Profiler.ProfileAsync(this.ProfilerStatName, () =>
            {
                StepAsync().Wait();
            });
        }
        catch (Exception ex)
        {
            // TODO: Log
        }
        
        // Update the stats.
        await this._statsSemaphore.WaitAsync();
        this.TicksCompleted += 1;
        this._tickRunning = false;
        this._statsSemaphore.Release();
    }

    /// <summary>
    /// Starts the loop.
    /// </summary>
    public void Start()
    {
        this._timer.Start();
    }

    /// <summary>
    /// Stops the loop.
    /// </summary>
    public void Stop()
    {
        this._timer.Stop();
    }

    /// <summary>
    /// Runs a step in the loop.
    /// </summary>
    public abstract Task StepAsync();
}