using System.Threading.Tasks;

namespace Enigma.Core.Shim.Output;

public interface IClipboard
{
    /// <summary>
    /// Sets the clipboard of the system.
    /// </summary>
    /// <param name="text">Text to put in the clipboard.</param>
    public Task SetTextAsync(string text);
}