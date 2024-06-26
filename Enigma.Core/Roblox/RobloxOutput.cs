using System.Diagnostics;
using System.Threading.Tasks;
using Enigma.Core.Diagnostic;
using Enigma.Core.OpenVr.Model;
using Enigma.Core.Shim.Output;
using Enigma.Core.Shim.Window;
using InputSimulatorStandard.Native;
using IClipboard = Enigma.Core.Shim.Output.IClipboard;

namespace Enigma.Core.Roblox;

public class RobloxOutput
{
    /// <summary>
    /// Interval in milliseconds between sending heartbeat keys.
    /// </summary>
    public const int HeartbeatIntervalMilliseconds = 250;

    /// <summary>
    /// The last data that was sent to Roblox.
    /// </summary>
    public string LastData { get; private set; } = "";

    /// <summary>
    /// The last data that was requested to be sent to Roblox.
    /// This is not guaranteed to be the last data that was successfully sent.
    /// </summary>
    public string LastRequestedData { get; private set; } = "";
    
    /// <summary>
    /// Keyboard to send key inputs with.
    /// </summary>
    private readonly IKeyboard _keyboard;
    
    /// <summary>
    /// Clipboard to send data with.
    /// </summary>
    private readonly IClipboard _clipboard;

    /// <summary>
    /// Window state to check if Roblox is focused.
    /// </summary>
    private readonly BaseWindowState _windowState;

    /// <summary>
    /// Stopwatch to occasionally send a heartbeat key press.
    /// Roblox can't seem to consistently detect keys like F13 staying down.
    /// </summary>
    private readonly Stopwatch _heartbeatStopwatch = new Stopwatch();

    /// <summary>
    /// Creates a Roblox output.
    /// </summary>
    /// <param name="keyboard">Keyboard to use.</param>
    /// <param name="clipboard">Clipboard to use.</param>
    /// <param name="windowState">Window state to use.</param>
    public RobloxOutput(IKeyboard keyboard, IClipboard clipboard, BaseWindowState windowState)
    {
        this._keyboard = keyboard;
        this._clipboard = clipboard;
        this._windowState = windowState;
    }

    /// <summary>
    /// Pushes a string of text to the Roblox client.
    /// Due to the method of sending data, it is not guaranteed to work, even if true is returned since some messages
    /// will paste after the previous one instead of replacing it.
    /// </summary>
    /// <param name="data">String data to push.</param>
    /// <returns>Whether the data was pushed to the client.</returns>
    public async Task<bool> PushDataAsync(string data)
    {
        // Return if the data is unchanged.
        // true is returned to act as if it was successful, even though nothing changed.
        if (data == this.LastData)
        {
            return true;
        }
        this.LastRequestedData = data;
        
        // Return if the window is not focused.
        if (!this._windowState.IsRobloxFocused())
        {
            if (this._heartbeatStopwatch.IsRunning)
            {
                Logger.Info("Stopping data sending to Roblox client.");
            }
            this._heartbeatStopwatch.Stop();
            return false;
        }
        
        // Send the heartbeat key.
        if (!this._heartbeatStopwatch.IsRunning)
        {
            this._heartbeatStopwatch.Start();
            this._keyboard.KeyPress(VirtualKeyCode.F13);
            Logger.Info("Starting data sending to Roblox client.");
        }
        else if (this._heartbeatStopwatch.ElapsedMilliseconds >= HeartbeatIntervalMilliseconds)
        {
            this._heartbeatStopwatch.Restart();
            this._keyboard.KeyPress(VirtualKeyCode.F13);
            Logger.Trace("Performing data sending heartbeat to Roblox client.");
        }
        
        // Set the clipboard and send the inputs to update the TextBox.
        var dataSent = false;
        await this._clipboard.SetTextAsync(data);
        if (!this._windowState.IsRobloxFocused())
        {
            Logger.Info("Stopping data sending to Roblox client.");
            return false;
        }
        this._keyboard.KeyDown(VirtualKeyCode.LCONTROL);
        this._keyboard.KeyPress(VirtualKeyCode.VK_A);
        if (this._windowState.IsRobloxFocused())
        {
            this._keyboard.KeyPress(VirtualKeyCode.VK_V);
            dataSent = true;
            this.LastData = data;
        }
        this._keyboard.KeyUp(VirtualKeyCode.LCONTROL);
        return dataSent;
    }

    /// <summary>
    /// Pushes a list of tracker inputs to the Roblox client.
    /// </summary>
    /// <param name="trackerInputs">Tracker input data to push.</param>
    public async Task<bool> PushTrackersAsync(TrackerInputList trackerInputs)
    {
        return await this.PushDataAsync(trackerInputs.Serialize());
    }
}