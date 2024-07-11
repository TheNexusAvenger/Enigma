using System.Collections.Generic;
using Valve.VR;

namespace Enigma.Core.OpenVr.Model;

public class OpenVrDevice
{
    /// <summary>
    /// Id of the device.
    /// </summary>
    public uint DeviceId { get; set; }

    /// <summary>
    /// Hardware id of the device.
    /// </summary>
    public string HardwareId { get; set; } = null!;
    
    /// <summary>
    /// Type of the device.
    /// </summary>
    public ETrackedDeviceClass DeviceType { get; set; }

    /// <summary>
    /// Role that is guessed from the properties.
    /// </summary>
    public TrackerRole GuessedRole { get; set; } = TrackerRole.None;
    
    /// <summary>
    /// Role that is provided by SteamVR.
    /// </summary>
    public TrackerRole SteamVrRole { get; set; } = TrackerRole.None;

    /// <summary>
    /// String properties from the device.
    /// </summary>
    public Dictionary<ETrackedDeviceProperty, string> StringProperties { get; set; } = new Dictionary<ETrackedDeviceProperty, string>();
}