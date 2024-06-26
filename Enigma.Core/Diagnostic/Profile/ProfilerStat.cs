namespace Enigma.Core.Diagnostic.Profile;

public class ProfilerStat
{
    /// <summary>
    /// Name of the stat.
    /// </summary>
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// Total number of times the stat has occured.
    /// </summary>
    public int TotalEvents { get; set; }
    
    /// <summary>
    /// Average time (in milliseconds) the action took to complete.
    /// </summary>
    public float AverageTime { get; set; }
}