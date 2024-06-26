using System.Threading.Tasks;
using Enigma.Core.Diagnostic;
using Enigma.Core.OpenVr;
using Enigma.Core.Roblox;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Enigma.Core.Web;

public class WebServer
{
    /// <summary>
    /// Port used for the web server.
    /// </summary>
    public const ushort Port = 52821;

    /// <summary>
    /// Roblox Studio state to set when the app connects.
    /// </summary>
    private readonly RobloxStudioState _robloxStudioState;

    /// <summary>
    /// Handler for OpenVR inputs.
    /// </summary>
    private readonly OpenVrInputs _openVrInputs;

    /// <summary>
    /// Creates a web server.
    /// </summary>
    /// <param name="robloxStudioState">Roblox Studio state to set when the app connects.</param>
    /// <param name="openVrInputs">Handler for OpenVR inputs.</param>
    public WebServer(RobloxStudioState robloxStudioState, OpenVrInputs openVrInputs)
    {
        this._robloxStudioState = robloxStudioState;
        this._openVrInputs = openVrInputs;
    }
    
    /// <summary>
    /// Starts the web server.
    /// </summary>
    /// <param name="addNexusLogging">Whether to add the logger for ASP.NET.</param>
    public void Start(bool addNexusLogging)
    {
        // Create the app builder with custom logging.
        var builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        if (addNexusLogging)
        {
            builder.Logging.AddProvider(Logger.NexusLogger);
        }
        
        // Set up custom exception handling.
        var app = builder.Build();
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandlerPathFeature != null)
                {
                    Logger.Error($"An exception occurred processing {context.Request.Method} {context.Request.Path}\n{exceptionHandlerPathFeature.Error}");
                }
                return Task.CompletedTask;
            });
        });
        
        // Build the API.
        var enigmaApi = app.MapGroup("/enigma");
        enigmaApi.MapGet("/status", () => "UP");
        enigmaApi.MapGet("/data", () =>
        {
            this._robloxStudioState.HeartbeatSent();
            return this._openVrInputs.GetInputs().Serialize();
        });

        // Run the server in the background.
        Task.Run(() =>
        {
            app.Run($"http://localhost:{Port}");
        });
    }
}