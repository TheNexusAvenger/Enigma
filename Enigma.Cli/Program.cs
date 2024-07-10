using System;
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
        // Check for updates in the background.
        var appInstances = new AppInstances();
        var _ = appInstances.UpdateCheck.CheckForUpdatesAsync();
        
        // Wait for OpenVR.
        try
        {
            await appInstances.OpenVrInputs.InitializeOpenVrAsync();
        }
        catch (DllNotFoundException)
        {
            await Logger.WaitForCompletionAsync();
            Environment.Exit(-1);
        }
        
        // Start the application.
        appInstances.SteamVrSettingsState.ConnectReloading();
        var webServerTask = appInstances.WebServer.StartAsync(this.AspNetLoggingEnabled);
        appInstances.RobloxOutputLoop.Start();
        appInstances.LogOutputLoop.Start();
        Logger.Info("Started Enigma. Make sure a Roblox client or Roblox Studio window is focused.");
    
        // Wait for the web server to exit.
        await webServerTask;
    }
}