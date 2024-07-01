using System.Threading;
using System.Threading.Tasks;
using Enigma.Core.Diagnostic;
using Enigma.Core.Program;

namespace Enigma.Cli;

public class Program : BaseProgram
{
    /// <summary>
    /// Runs the program.
    /// </summary>
    /// <param name="args">Arguments from the command line.</param>
    public static async Task<int> Main(string[] args)
    {
        var program = new Program();
        return await program.RunMainAsync(args);
    }

    /// <summary>
    /// Runs the application after the common setup has completed.
    /// </summary>
    public override async Task RunAsync()
    {
        // Start the application.
        var appInstances = new AppInstances();
        await appInstances.OpenVrInputs.InitializeOpenVrAsync();
        appInstances.SteamVrSettingsState.ConnectReloading();
        var webServerTask = appInstances.WebServer.StartAsync(this.AspNetLoggingEnabled);
        appInstances.RobloxOutputLoop.Start();
        appInstances.LogOutputLoop.Start();
        Logger.Info("Started Enigma. Make sure a Roblox client or Roblox Studio window is focused.");
        
        // Wait for the web server to exit.
        await webServerTask;
    }
}