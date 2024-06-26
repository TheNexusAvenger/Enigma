﻿using System.Diagnostics;
using System.Threading.Tasks;
using Enigma.Core.Diagnostic;
using Enigma.Core.OpenVr.Model;
using Enigma.Core.Shim.Output;
using Enigma.Core.Shim.Window;
using InputSimulatorStandard.Native;
using Clipboard = Enigma.Core.Shim.Output.Clipboard;
using IClipboard = Enigma.Core.Shim.Output.IClipboard;

namespace Enigma.Core.Roblox;

public class RobloxOutput
{
    /// <summary>
    /// Interval in milliseconds between sending heartbeat keys.
    /// </summary>
    public const int HeartbeatIntervalMilliseconds = 250;
    
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
    /// </summary>
    /// <param name="data">String data to push.</param>
    public async Task PushDataAsync(string data)
    {
        // Return if the window is not focused.
        if (!this._windowState.IsRobloxFocused())
        {
            this._heartbeatStopwatch.Stop();
            Logger.Info("Stopping data sending to Roblox client.");
            return;
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
        await this._clipboard.SetTextAsync(data);
        if (!this._windowState.IsRobloxFocused())
        {
            Logger.Info("Stopping data sending to Roblox client.");
            return;
        }
        this._keyboard.KeyDown(VirtualKeyCode.LCONTROL);
        this._keyboard.KeyPress(VirtualKeyCode.VK_A);
        if (this._windowState.IsRobloxFocused())
        {
            this._keyboard.KeyPress(VirtualKeyCode.VK_V);
        }
        this._keyboard.KeyUp(VirtualKeyCode.LCONTROL);
    }

    /// <summary>
    /// Pushes a list of tracker inputs to the Roblox client.
    /// </summary>
    /// <param name="trackerInputs">Tracker input data to push.</param>
    public async Task PushTrackersAsync(TrackerInputList trackerInputs)
    {
        await this.PushDataAsync(trackerInputs.Serialize());
    }
}