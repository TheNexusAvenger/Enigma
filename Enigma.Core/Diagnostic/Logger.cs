using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nexus.Logging.Attribute;
using Nexus.Logging.Output;

namespace Enigma.Core.Diagnostic;

public class Logger
{
    /// <summary>
    /// Static instance of the logger.
    /// </summary>
    public static readonly Nexus.Logging.Logger NexusLogger = new Nexus.Logging.Logger();

    /// <summary>
    /// Set of the messages that have been logged that are only meant to be logged once.
    /// </summary>
    private static readonly HashSet<string> _loggedOnceMessages = new HashSet<string>();

    /// <summary>
    /// Static instance of the console output.
    /// </summary>
    private static readonly ConsoleOutput ConsoleOutput = new ConsoleOutput()
    {
        IncludeDate = true,
        NamespaceWhitelist = new List<string>() { "Enigma" },
        MinimumLevel = LogLevel.Information,
    };
    
    /// <summary>
    /// Sets up the logging.
    /// </summary>
    static Logger()
    {
        NexusLogger.Outputs.Add(ConsoleOutput);
    }

    /// <summary>
    /// Sets the minimum level for logging.
    /// </summary>
    public static void SetMinimumLogLevel(LogLevel logLevel)
    {
        ConsoleOutput.MinimumLevel = logLevel;
    }
    
    /// <summary>
    /// Logs a message as a Trace level.
    /// </summary>
    /// <param name="content">Content to log. Can be an object, like an exception.</param>
    [LogTraceIgnore]
    public static void Trace(object content)
    {
        NexusLogger.Trace(content);
    }
    
    /// <summary>
    /// Logs a message as a Debug level.
    /// </summary>
    /// <param name="content">Content to log. Can be an object, like an exception.</param>
    [LogTraceIgnore]
    public static void Debug(object content)
    {
        NexusLogger.Debug(content);
    }

    /// <summary>
    /// Logs a message as an Information level.
    /// </summary>
    /// <param name="content">Content to log. Can be an object, like an exception.</param>
    [LogTraceIgnore]
    public static void Info(object content)
    {
        NexusLogger.Info(content);
    }

    /// <summary>
    /// Logs a message as a Warning level.
    /// </summary>
    /// <param name="content">Content to log. Can be an object, like an exception.</param>
    [LogTraceIgnore]
    public static void Warn(object content)
    {
        NexusLogger.Warn(content);
    }

    /// <summary>
    /// Logs a message as a Error level.
    /// </summary>
    /// <param name="content">Content to log. Can be an object, like an exception.</param>
    [LogTraceIgnore]
    public static void Error(object content)
    {
        NexusLogger.Error(content);
    }

    /// <summary>
    /// Logs a message only once. Repeated attempts will be ignored.
    /// </summary>
    /// <param name="logLevel">Log level to log with.</param>
    /// <param name="message">Message to log once.</param>
    [LogTraceIgnore]
    public static void LogOnce(LogLevel logLevel, string message)
    {
        if (!_loggedOnceMessages.Add(message)) return;
        NexusLogger.Log(message, logLevel);
    }
    
    /// <summary>
    /// Waits for all loggers to finish processing logs.
    /// </summary>
    public static async Task WaitForCompletionAsync()
    {
        await NexusLogger.WaitForCompletionAsync();
    }
}