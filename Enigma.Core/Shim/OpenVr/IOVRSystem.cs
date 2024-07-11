using System.Text;
using Valve.VR;

namespace Enigma.Core.Shim.OpenVr;

public interface IOVRSystem
{
    /// <summary>
    /// Gets the pose of an array of tracked devices.
    /// </summary>
    /// <param name="eOrigin">Origin to read the pose from.</param>
    /// <param name="fPredictedSecondsToPhotonsFromNow">Offset (in seconds) to extrapolate.</param>
    /// <param name="pTrackedDevicePoseArray">Array to put the poses in.</param>
    public void GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin eOrigin, float fPredictedSecondsToPhotonsFromNow, TrackedDevicePose_t[] pTrackedDevicePoseArray);
    
    /// <summary>
    /// Reads a property from a device.
    /// </summary>
    /// <param name="unDeviceIndex">Index of the device to read the property of.</param>
    /// <param name="prop">Property to get the value of.</param>
    /// <param name="pchValue">StringBuffer to write the value to.</param>
    /// <param name="unBufferSize">Size of the buffer to write to.</param>
    /// <param name="pError">Reference to the error enum.</param>
    public void GetStringTrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, StringBuilder pchValue, uint unBufferSize, ref ETrackedPropertyError pError);

    /// <summary>
    /// Returns the class of the device.
    /// </summary>
    /// <param name="unDeviceIndex">Index of the device to check.</param>
    /// <returns>The class of the device.</returns>
    public ETrackedDeviceClass GetTrackedDeviceClass(uint unDeviceIndex);

    /// <summary>
    /// Returns if a device is connected.
    /// </summary>
    /// <param name="unDeviceIndex">Index of the device to check.</param>
    /// <returns>Whether the device is connected.</returns>
    public bool IsTrackedDeviceConnected(uint unDeviceIndex);
}