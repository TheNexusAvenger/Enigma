using System.Collections.Generic;
using System.Text;
using Enigma.Core.Shim.OpenVr;
using Valve.VR;

namespace Enigma.Core.Test.TestShim;

public class TestOpenVrDevice
{
    /// <summary>
    /// Device class of the device.
    /// </summary>
    public ETrackedDeviceClass DeviceClass { get; set; } = ETrackedDeviceClass.GenericTracker;

    /// <summary>
    /// Whether the device is connected.
    /// </summary>
    public bool Connected { get; set; } = true;
    
    /// <summary>
    /// Properties of the device.
    /// </summary>
    public Dictionary<ETrackedDeviceProperty, string> Properties = new Dictionary<ETrackedDeviceProperty, string>();
}

public class TestOVRSystem : IOVRSystem
{
    /// <summary>
    /// List of the test devices.
    /// </summary>
    public readonly List<TestOpenVrDevice> Devices = new List<TestOpenVrDevice>();
    
    /// <summary>
    /// Gets the pose of an array of tracked devices.
    /// </summary>
    /// <param name="eOrigin">Origin to read the pose from.</param>
    /// <param name="fPredictedSecondsToPhotonsFromNow">Offset (in seconds) to extrapolate.</param>
    /// <param name="pTrackedDevicePoseArray">Array to put the poses in.</param>
    public void GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin eOrigin, float fPredictedSecondsToPhotonsFromNow, TrackedDevicePose_t[] pTrackedDevicePoseArray)
    {
        for (var i = 0; i < this.Devices.Count; i++)
        {
            pTrackedDevicePoseArray[i] = new TrackedDevicePose_t()
            {
                mDeviceToAbsoluteTracking = new HmdMatrix34_t(),
                vVelocity = new HmdVector3_t()
                {
                    v0 = i,
                    v1 = i + 1,
                    v2 = i + 2,
                },
            };
        }
    }

    /// <summary>
    /// Reads a property from a device.
    /// </summary>
    /// <param name="unDeviceIndex">Index of the device to read the property of.</param>
    /// <param name="prop">Property to get the value of.</param>
    /// <param name="pchValue">StringBuffer to write the value to.</param>
    /// <param name="unBufferSize">Size of the buffer to write to.</param>
    /// <param name="pError">Reference to the error enum.</param>
    public void GetStringTrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, StringBuilder pchValue, uint unBufferSize, ref ETrackedPropertyError pError)
    {
        // Set the status as an error if the device doesn't exist.
        if (unDeviceIndex >= this.Devices.Count)
        {
            pError = ETrackedPropertyError.TrackedProp_InvalidDevice;
            return;
        }
        
        // Set the status as an error if the property isn't set.
        var device = this.Devices[(int) unDeviceIndex];
        if (!device.Properties.TryGetValue(prop, out var propertyValue))
        {
            pError = ETrackedPropertyError.TrackedProp_ValueNotProvidedByDevice;
            return;
        }
        
        // Set the value.
        pchValue.Append(propertyValue);
        pError = ETrackedPropertyError.TrackedProp_Success;
    }

    /// <summary>
    /// Returns the class of the device.
    /// </summary>
    /// <param name="unDeviceIndex">Index of the device to check.</param>
    /// <returns>The class of the device.</returns>
    public ETrackedDeviceClass GetTrackedDeviceClass(uint unDeviceIndex)
    {
        return unDeviceIndex >= this.Devices.Count ? ETrackedDeviceClass.Invalid : this.Devices[(int) unDeviceIndex].DeviceClass;
    }

    /// <summary>
    /// Returns if a device is connected.
    /// </summary>
    /// <param name="unDeviceIndex">Index of the device to check.</param>
    /// <returns>Whether the device is connected.</returns>
    public bool IsTrackedDeviceConnected(uint unDeviceIndex)
    {
        return unDeviceIndex < this.Devices.Count && this.Devices[(int) unDeviceIndex].Connected;
    }
}