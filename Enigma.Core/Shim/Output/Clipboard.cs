using System.Threading.Tasks;
using TextCopy;

namespace Enigma.Core.Shim.Output;

public class Clipboard : IClipboard
{
    /// <summary>
    /// Sets the clipboard of the system.
    /// </summary>
    /// <param name="text">Text to put in the clipboard.</param>
    public async Task SetTextAsync(string text)
    {
        await ClipboardService.SetTextAsync(text);
    }
}