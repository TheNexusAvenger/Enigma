using System.Numerics;
using Valve.VR;

namespace Enigma.Core.OpenVr.Model;

public class TrackerInput
{
    /// <summary>
    /// Id of the device.
    /// </summary>
    public uint DeviceId { get; set; }

    /// <summary>
    /// Type of the device.
    /// </summary>
    public ETrackedDeviceClass DeviceType { get; set; }
    
    /// <summary>
    /// Role of the tracker.
    /// </summary>
    public TrackerRole TrackerRole { get; set; }

    /// <summary>
    /// Position of the tracker input relative to the headset.
    /// </summary>
    public Vector3 Position { get; set; }
    
    /// <summary>
    /// Rotation of the tracker input relative to the headset.
    /// </summary>
    public Quaternion Rotation { get; set; }

    /// <summary>
    /// Velocity of the tracker input relative to the headset.
    /// </summary>
    public Vector3 Velocity { get; set; }
}