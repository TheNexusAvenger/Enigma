using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
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
    public static readonly Option<bool> HttpDebugLoggingOption = new Option<bool>("--debug-http", "Enables logging for the ASP.NET Core HTTP server used by the Roblox Studio companion app.");
    
    /// <summary>
    /// Command option for masking OpenVR properties in list-devices.
    /// </summary>
    public static readonly Option<bool> MaskPropertiesOption = new Option<bool>("--masked", "Masks some properties that may or may not be sensitive.");

    /// <summary>
    /// Command for running the application.
    /// </summary>
    public static readonly Command RunCommand = new Command("run", "Runs the Enigma application.");

    /// <summary>
    /// Command for listing OpenVR devices.
    /// </summary>
    public static readonly Command ListDevicesCommand = new Command("list-devices", "Lists the devices that are currently available to OpenVR.");

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
        // Add the options to the commands.
        RunCommand.AddOption(DebugOption);
        RunCommand.AddOption(TraceOption);
        RunCommand.AddOption(HttpDebugLoggingOption);
        RunCommand.SetHandler(this.RunApplicationAsync);
        ListDevicesCommand.AddOption(DebugOption);
        ListDevicesCommand.AddOption(TraceOption);
        ListDevicesCommand.AddOption(MaskPropertiesOption);
        ListDevicesCommand.SetHandler(this.ListDevicesAsync);
        
        // Create the root command.
        // The root command also functions as the run command.
        var rootCommand = new RootCommand(description: "Provides SteamVR tracker data to the Roblox client.");
        rootCommand.AddOption(DebugOption);
        rootCommand.AddOption(TraceOption);
        rootCommand.AddOption(HttpDebugLoggingOption);
        rootCommand.AddCommand(RunCommand);
        rootCommand.AddCommand(ListDevicesCommand);
        rootCommand.SetHandler(this.RunApplicationAsync);
        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// Prepares the application for running.
    /// </summary>
    /// <param name="invocationContext">Context for the command line options.</param>
    private void PrepareApplication(InvocationContext invocationContext)
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
    }

    /// <summary>
    /// Runs the application with parsed command line arguments.
    /// </summary>
    /// <param name="invocationContext">Context for the command line options.</param>
    private async Task RunApplicationAsync(InvocationContext invocationContext)
    {
        // Prepare the logging.
        this.PrepareApplication(invocationContext);
        
        // Check if logging should be added to ASP.NET.
        if (invocationContext.ParseResult.GetValueForOption(HttpDebugLoggingOption))
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
    /// 
    /// </summary>
    /// <param name="invocationContext">Context for the command line options.</param>
    private async Task ListDevicesAsync(InvocationContext invocationContext)
    {
        // Prepare the logging.
        this.PrepareApplication(invocationContext);
        
        // Wait for OpenVR to be started.
        var appInstances = new AppInstances();
        try
        {
            await appInstances.OpenVrInputs.InitializeOpenVrAsync();
        }
        catch (DllNotFoundException)
        {
            await Logger.WaitForCompletionAsync();
            Environment.Exit(-1);
        }
        await appInstances.SteamVrSettingsState.ReloadSettingsAsync();
        Logger.Info("Listing OpenVR devices.");
        
        // Get and list the OpenVR devices.
        var maskData = invocationContext.ParseResult.GetValueForOption(MaskPropertiesOption);
        var devices = appInstances.OpenVrInputs.ListDevices();
        var devicesOutput = new StringBuilder();
        devicesOutput.Append($"OpenVR devices ({devices.Count}):");
        foreach (var device in devices)
        {
            var hardwareIdToShow = (maskData ? OpenVrPropertyMasker.MaskDeviceId(device.HardwareId) : device.HardwareId);
            devicesOutput.Append($"\n| [{device.DeviceId}] {hardwareIdToShow} ({device.DeviceType})");
            foreach (var (propertyName, propertyValue) in device.StringProperties)
            {
                var valueToShow = (maskData ? OpenVrPropertyMasker.MaskProperty(propertyName, propertyValue) : propertyValue);
                devicesOutput.Append($"\n|   {propertyName}: \"{valueToShow}\"");
            }
        }
        Logger.Info(devicesOutput);

        // List all the SteamVR tracker roles.
        var trackerRoles = appInstances.SteamVrSettingsState.GetAllTrackerRoles();
        var trackersOutputs = new StringBuilder();
        trackersOutputs.Append($"SteamVR tracker roles ({trackerRoles.Count}):");
        foreach (var (trackerName, trackerRole) in trackerRoles)
        {
            var trackerNameToShow = (maskData ? OpenVrPropertyMasker.MaskDeviceId(trackerName) : trackerName);
            trackersOutputs.Append($"\n| {trackerNameToShow}: {trackerRole}");
        }
        Logger.Info(trackersOutputs);
        
        // Wait for the logging to complete.
        await Logger.WaitForCompletionAsync();
    }

    /// <summary>
    /// Runs the application after the common setup has completed.
    /// </summary>
    public abstract Task RunAsync();
}