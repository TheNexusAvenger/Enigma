using System.Threading.Tasks;
using Enigma.Core.Diagnostic.Profile;
using Enigma.Core.OpenVr;
using Enigma.Core.Roblox;

namespace Enigma.Core.Loop;

public class RobloxOutputLoop : BaseLoop
{
    /// <summary>
    /// Input handler for OpenVR.
    /// </summary>
    private readonly OpenVrInputs _openVrInputs;

    /// <summary>
    /// Roblox keyboard output.
    /// </summary>
    private readonly RobloxOutput _robloxOutput;

    /// <summary>
    /// Creates a Roblox output loop.
    /// </summary>
    /// <param name="openVrInputs">Input handler for OpenVR.</param>
    /// <param name="robloxOutput">Roblox keyboard output.</param>
    public RobloxOutputLoop(OpenVrInputs openVrInputs, RobloxOutput robloxOutput) : base(15)
    {
        this._openVrInputs = openVrInputs;
        this._robloxOutput = robloxOutput;
    }
    
    /// <summary>
    /// Runs a step in the loop.
    /// </summary>
    public override async Task StepAsync()
    {
        // Get the OpenVR inputs.
        var newInputs = await Profiler.ProfileAsync("OpenVRGetInputs", () => this._openVrInputs.GetInputs());
        
        // Send the OpenVR inputs to the client.
        await Profiler.ProfileAsync("PushTrackerData", async () =>
        {
            await this._robloxOutput.PushTrackersAsync(newInputs);
        });
    }
}