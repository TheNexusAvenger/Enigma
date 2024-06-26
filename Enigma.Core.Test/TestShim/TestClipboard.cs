using System.Threading.Tasks;
using NUnit.Framework;
using IClipboard = Enigma.Core.Shim.Output.IClipboard;

namespace Enigma.Core.Test.TestShim;

public class TestClipboard : IClipboard
{
    /// <summary>
    /// Current text in the clipboard.
    /// </summary>
    private string? _clipboard;
    
    /// <summary>
    /// Sets the clipboard of the system.
    /// </summary>
    /// <param name="text">Text to put in the clipboard.</param>
    public Task SetTextAsync(string text)
    {
        this._clipboard = text;
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Asserts the contents of the clipboard.
    /// </summary>
    /// <param name="text">Text to the clipboard to assert.</param>
    public void AssertClipboard(string? text)
    {
        Assert.That(this._clipboard, Is.EqualTo(text));
    }
}