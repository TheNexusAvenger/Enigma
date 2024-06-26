using System.CommandLine.Invocation;
using System.Threading;
using System.Threading.Tasks;
using Enigma.Core;
using Enigma.Core.Diagnostic;
using Enigma.Core.Web;
using Microsoft.Extensions.Logging;

namespace Enigma.Cli;

using System.CommandLine;

public class Program
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
    /// Runs the program.
    /// </summary>
    /// <param name="args">Arguments from the command line.</param>
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand(description: "Provides SteamVR tracker data to the Roblox client.");
        rootCommand.AddOption(DebugOption);
        rootCommand.AddOption(TraceOption);
        rootCommand.SetHandler(Run);
        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// Runs the application with parsed command line arguments.
    /// </summary>
    /// <param name="invocationContext">Context for the command line options.</param>
    public static async Task Run(InvocationContext invocationContext)
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
        
        // Start the application.
        var appInstances = new AppInstances();
        await appInstances.OpenVrInputs.InitializeOpenVrAsync();
        appInstances.WebServer.Start();
        appInstances.RobloxOutputLoop.Start();
        appInstances.LogOutputLoop.Start();
        Logger.Info("Started Enigma. Make sure a Roblox client or Roblox Studio window is focused.");
        
        // Keep the application alive.
        new CancellationToken().WaitHandle.WaitOne();
    }
}