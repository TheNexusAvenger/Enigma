using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Enigma.Core.Diagnostic;
using Enigma.Core.Roblox;
using Microsoft.Extensions.Logging;

namespace Enigma.Core.Program;

public abstract class BaseProgram
{
    /// <summary>
    /// Command option for enabling debug logging.
    /// </summary>
    public static readonly Option<bool> DebugOption = new Option<bool>("--debug", "Enables debug logging.");
    
    /// <summary>
    /// Command option for enabling trace and debug logging.
    /// </summary>
    public static readonly Option<bool> TraceOption = new Option<bool>("--trace", "Enables debug and trace logging.");
    
    /// <summary>
    /// Command option for enabling logging for ASP.NET Core.
    /// </summary>
    public static readonly Option<bool> HttpDebugLogging = new Option<bool>("--debug-http", "Enables logging for the ASP.NET Core HTTP server used by the Roblox Studio companion app.");

    /// <summary>
    /// Whether logging for ASP.NET Core should be enabled.
    /// </summary>
    public bool AspNetLoggingEnabled { get; set; } = false;
    
    /// <summary>
    /// Runs the program.
    /// </summary>
    /// <param name="args">Arguments from the command line.</param>
    public async Task<int> RunMainAsync(string[] args)
    {
        var rootCommand = new RootCommand(description: "Provides SteamVR tracker data to the Roblox client.");
        rootCommand.AddOption(DebugOption);
        rootCommand.AddOption(TraceOption);
        rootCommand.AddOption(HttpDebugLogging);
        rootCommand.SetHandler(this.RunApplicationAsync);
        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// Runs the application with parsed command line arguments.
    /// </summary>
    /// <param name="invocationContext">Context for the command line options.</param>
    private async Task RunApplicationAsync(InvocationContext invocationContext)
    {
        // Set the log level.
        if (invocationContext.ParseResult.GetValueForOption(TraceOption))
        {
            Logger.SetMinimumLogLevel(LogLevel.Trace);
            Logger.Debug("Enabled trace and debug logging.");
        }
        else if (invocationContext.ParseResult.GetValueForOption(DebugOption))
        {
            Logger.SetMinimumLogLevel(LogLevel.Debug);
            Logger.Debug("Enabled debug logging.");
        }
        
        // Check if logging should be added to ASP.NET.
        if (invocationContext.ParseResult.GetValueForOption(HttpDebugLogging))
        {
            this.AspNetLoggingEnabled = true;
            Logger.Debug("Enabled ASP.NET Core logging.");
        }
        
        // Copy the plugin.
        await RobloxPlugins.CopyPluginAsync();
        
        // Run the program.
        await this.RunAsync();
    }

    /// <summary>
    /// Runs the application after the common setup has completed.
    /// </summary>
    public abstract Task RunAsync();
}