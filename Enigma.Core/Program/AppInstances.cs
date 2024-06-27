using Enigma.Core.Loop;
using Enigma.Core.OpenVr;
using Enigma.Core.Roblox;
using Enigma.Core.Shim.Output;
using Enigma.Core.Shim.Window;
using Enigma.Core.Web;

namespace Enigma.Core.Program;

public class AppInstances
{
    /// <summary>
    /// Clipboard instance used by the application.
    /// </summary>
    public readonly IClipboard Clipboard;

    /// <summary>
    /// Keyboard instance used by the application.
    /// </summary>
    public readonly IKeyboard Keyboard;

    /// <summary>
    /// Roblox Studio state instance used by the application.
    /// </summary>
    public readonly RobloxStudioState RobloxStudioState;

    /// <summary>
    /// Window state instance used by the application.
    /// </summary>
    public readonly BaseWindowState WindowState;

    /// <summary>
    /// Handler for reading OpenVR inputs.
    /// </summary>
    public readonly OpenVrInputs OpenVrInputs;

    /// <summary>
    /// Handler for pushing data to Roblox.
    /// </summary>
    public readonly RobloxOutput RobloxOutput;

    /// <summary>
    /// Web server instance used by the application.
    /// </summary>
    public readonly WebServer WebServer;
    
    /// <summary>
    /// Log output loop instance used by the application.
    /// </summary>
    public readonly RobloxOutputLoop RobloxOutputLoop;
    
    /// <summary>
    /// Log output loop instance used by the application.
    /// </summary>
    public readonly LogOutputLoop LogOutputLoop;

    /// <summary>
    /// Initializes the app instances.
    /// </summary>
    public AppInstances()
    {
        // Create the shims.
        this.Clipboard = new Clipboard();
        this.Keyboard = new Keyboard();
        this.RobloxStudioState = new RobloxStudioState();
        this.WindowState = new WindowsWindowState(this.RobloxStudioState);
        
        // Create the inputs and outputs.
        this.OpenVrInputs = new OpenVrInputs();
        this.RobloxOutput = new RobloxOutput(this.Keyboard, this.Clipboard, this.WindowState);
        this.WebServer = new WebServer(this.RobloxStudioState, this.RobloxOutput);
        
        // Create the loops.
        this.RobloxOutputLoop = new RobloxOutputLoop(this.OpenVrInputs, this.RobloxOutput);
        this.LogOutputLoop = new LogOutputLoop(this.RobloxOutputLoop);
    }
}