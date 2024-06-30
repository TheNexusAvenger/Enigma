using System;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Enigma.Core.Diagnostic;
using Enigma.Core.Extension;
using Enigma.Core.OpenVr.Model;
using OVRSharp;
using OVRSharp.Exceptions;
using OVRSharp.Math;
using Valve.VR;

namespace Enigma.Core.OpenVr;

public class OpenVrInputs
{
    /// <summary>
    /// Total number of OpenVR devices to check for.
    /// TODO: Hard-coding this seems bad. Maybe find a way to dynamically determine this?
    /// </summary>
    public const int DevicesToCheck = 32;

    /// <summary>
    /// State of the SteamVR settings to read tracker roles.
    /// </summary>
    private readonly SteamVrSettingsState _steamVrSettingsState;
    
    /// <summary>
    /// Instance of the OpenVR system for reading inputs.
    /// </summary>
    private CVRSystem? _ovrSystem;

    /// <summary>
    /// Creates a OpenVR inputs reader.
    /// </summary>
    /// <param name="steamVrSettingsState">State of the SteamVR settings to read tracker roles.</param>
    public OpenVrInputs(SteamVrSettingsState steamVrSettingsState)
    {
        this._steamVrSettingsState = steamVrSettingsState;
    }

    /// <summary>
    /// Initializes OpenVR. Yields for it to be initialized.
    /// </summary>
    public async Task InitializeOpenVrAsync()
    {
        var waitMessageLogged = false;
        while (this._ovrSystem == null)
        {
            try
            {
                // Initialize OpenVR.
                this._ovrSystem = new Application(Application.ApplicationType.Background).OVRSystem;
            }
            catch (OpenVRSystemException<EVRInitError>)
            {
                // Log the message if it is the first time, and wait to retry.
                if (waitMessageLogged == false)
                {
                    waitMessageLogged = true;
                    Logger.Info("OpenVR (SteamVR) is not running or has no hardware detected. Waiting for the headset to become detected.");
                }
                await Task.Delay(50);
            }
        }
        
        // Print after it loaded if OpenVR was not initially open.
        if (waitMessageLogged)
        {
            Logger.Debug("OpenVR headset detected.");
        }
    }

    /// <summary>
    /// Returns the value of a string property. Null is returned if the property had an error reading.
    /// </summary>
    /// <param name="trackerIndex">Index of the tracker to get the value of.</param>
    /// <param name="property">Property to try tor ead.</param>
    /// <returns>The value for the property, if it exists.</returns>
    private string? GetTrackerStringProperty(uint trackerIndex, ETrackedDeviceProperty property)
    {
        // Throw an exception if OpenVR is not initialized.
        if (this._ovrSystem == null)
        {
            throw new InvalidOperationException("OpenVR is not initialized.");
        }
        
        // Build and return the message if it was a success.
        var error = ETrackedPropertyError.TrackedProp_Success;
        var propertyBuilder = new StringBuilder();
        this._ovrSystem.GetStringTrackedDeviceProperty(trackerIndex, property, propertyBuilder, 128, ref error);
        return error == ETrackedPropertyError.TrackedProp_Success ? propertyBuilder.ToString() : null;
    }
    
    /// <summary>
    /// Returns the current OpenVR inputs for Roblox.
    /// </summary>
    /// <returns>The current OpenVR inputs for Roblox.</returns>
    public TrackerInputList GetInputs()
    {
        // Throw an exception if OpenVR is not initialized.
        if (this._ovrSystem == null)
        {
            throw new InvalidOperationException("OpenVR is not initialized.");
        }
        
        // Get the poses.
        var poseArray = new TrackedDevicePose_t[DevicesToCheck];
        this._ovrSystem.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, poseArray);
        
        // Get the ids.
        uint headsetId = 0;
        var trackerInputs = new TrackerInputList();
        for (uint i = 0; i < DevicesToCheck; i++)
        {
            var inputType = this._ovrSystem.GetTrackedDeviceClass(i);
            if (inputType == ETrackedDeviceClass.HMD)
            {
                // Store the index of the headset.
                headsetId = i;
            }
            else if (inputType == ETrackedDeviceClass.GenericTracker)
            {
                // Get the tracker information.
                if (!this._ovrSystem.IsTrackedDeviceConnected(i)) continue;
                var trackingSystem = this.GetTrackerStringProperty(i, ETrackedDeviceProperty.Prop_TrackingSystemName_String);
                if (trackingSystem == null) continue;
                var serialNumber = this.GetTrackerStringProperty(i, ETrackedDeviceProperty.Prop_SerialNumber_String);
                if (serialNumber == null) continue;
                var trackerRole = this._steamVrSettingsState.GetRole(trackingSystem, serialNumber);
                
                // Add the tracker information.
                trackerInputs.Add(new TrackerInput()
                {
                    DeviceId = i,
                    DeviceType = inputType,
                    TrackerRole = trackerRole,
                });
            }
        }
        
        // Get the headset pose.
        var headPose = poseArray[headsetId].mDeviceToAbsoluteTracking.ToMatrix4x4();
        var headRotation = Quaternion.CreateFromRotationMatrix(headPose).FlipHandedness();
        
        // Add the relative tracker positions.
        foreach (var input in trackerInputs)
        {
            var trackerPoseData = poseArray[input.DeviceId];
            var trackerPose = trackerPoseData.mDeviceToAbsoluteTracking.ToMatrix4x4();
            var trackerRotation = Quaternion.CreateFromRotationMatrix(trackerPose).FlipHandedness();
            var trackerVelocity = new Vector3(trackerPoseData.vVelocity.v0, trackerPoseData.vVelocity.v1, trackerPoseData.vVelocity.v2);
            input.Rotation = Quaternion.Inverse(headRotation) * trackerRotation;
            input.Position = Vector3.Transform(trackerPose.Translation - headPose.Translation, Quaternion.Inverse(headRotation)).ConvertMetersToFeet();
            input.Velocity = Vector3.Transform(trackerVelocity, Quaternion.Inverse(headRotation)).ConvertMetersToFeet();
        }
        
        // Return the inputs.
        return trackerInputs;
    }
}