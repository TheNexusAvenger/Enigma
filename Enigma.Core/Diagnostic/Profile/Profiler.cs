using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Enigma.Core.Diagnostic.Profile;

public class Profiler
{
    /// <summary>
    /// Semaphore for storing milliseconds.
    /// </summary>
    private static readonly SemaphoreSlim ProfileSemaphore = new SemaphoreSlim(1);

    /// <summary>
    /// Stats stored for the profiled actions.
    /// </summary>
    private static readonly Dictionary<string, ProfilerStat> Stats = new Dictionary<string, ProfilerStat>();

    /// <summary>
    /// Returns the current stat for a name, if it exists.
    /// </summary>
    /// <param name="name">Name of the stat to get.</param>
    /// <returns>The current stat, if it exists.</returns>
    public static async Task<ProfilerStat?> GetStatAsync(string name)
    {
        // Return null if there is no stat.
        await ProfileSemaphore.WaitAsync();
        if (!Stats.TryGetValue(name, out var existingStat))
        {
            ProfileSemaphore.Release();
            return null;
        }

        // Return a copy of the stat.
        var newStat = new ProfilerStat()
        {
            Name = existingStat.Name,
            TotalEvents = existingStat.TotalEvents,
            AverageTime = existingStat.AverageTime,
        };
        ProfileSemaphore.Release();
        return newStat;
    }

    /// <summary>
    /// Adds a stat.
    /// </summary>
    /// <param name="name">Name of the stat.</param>
    /// <param name="durationMilliseconds">Optional duration of the stat.</param>
    public static async Task AddStatAsync(string name, long durationMilliseconds = 0)
    {
        // Store the initial stat.
        await ProfileSemaphore.WaitAsync();
        if (!Stats.ContainsKey(name))
        {
            Stats[name] = new ProfilerStat()
            {
                Name = name,
            };
        }
        
        // Add the duration and total events.
        var stat = Stats[name];
        stat.AverageTime = ((stat.AverageTime * stat.TotalEvents) + durationMilliseconds) / (stat.TotalEvents + 1);
        stat.TotalEvents += 1;
        ProfileSemaphore.Release();
    }

    /// <summary>
    /// Profiles a task and stores how many milliseconds it took to complete.
    /// </summary>
    /// <param name="name">Name of the task to profile.</param>
    /// <param name="task">Task to profile.</param>
    public static async Task ProfileAsync(string name, Task task)
    {
        // Run the action.
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        await task;
        stopwatch.Stop();
        
        // Store the completion time in milliseconds.
        await AddStatAsync(name, stopwatch.ElapsedMilliseconds);
    }
    
    /// <summary>
    /// Profiles a task and stores how many milliseconds it took to complete.
    /// </summary>
    /// <param name="name">Name of the task to profile.</param>
    /// <param name="task">Task to profile.</param>
    public static async Task<T> ProfileAsync<T>(string name, Task<T> task)
    {
        T? result = default;
        await ProfileAsync(name, Task.Run(async () =>
        {
            result = await task;
        }));
        return result!;
    }
    
    /// <summary>
    /// Profiles an action and stores how many milliseconds it took to complete.
    /// </summary>
    /// <param name="name">Name of the action to profile.</param>
    /// <param name="action">Action to run.</param>
    public static async Task ProfileAsync(string name, Action action)
    {
        await ProfileAsync(name, Task.Run(action));
    }
    
    /// <summary>
    /// Profiles an action and stores how many milliseconds it took to complete.
    /// </summary>
    /// <param name="name">Name of the action to profile.</param>
    /// <param name="action">Action to run.</param>
    public static async Task<T> ProfileAsync<T>(string name, Func<T> action)
    {
        return await ProfileAsync(name, Task.Run(action));
    }
    
    /// <summary>
    /// Resets all current stats.
    /// </summary>
    public static async Task ResetAsync()
    {
        await ProfileSemaphore.WaitAsync();
        Stats.Clear();
        ProfileSemaphore.Release();
    }
}