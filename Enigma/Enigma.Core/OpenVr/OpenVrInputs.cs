using System.Numerics;
using Enigma.Core.Extension;
using Enigma.Core.OpenVr.Model;
using OVRSharp;
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
    /// Instance of the OpenVR system for reading inputs.
    /// </summary>
    private readonly CVRSystem _ovrSystem;

    /// <summary>
    /// Creates an OpenVR input handler.
    /// </summary>
    public OpenVrInputs()
    {
        // TODO: Move out of initialization to allow app to start without SteamVR running.
        // TODO: What happens when SteamVR stops?
        var app = new Application(Application.ApplicationType.Background);
        this._ovrSystem = app.OVRSystem;
    }
    
    /// <summary>
    /// Returns the current OpenVR inputs for Roblox.
    /// </summary>
    /// <returns>The current OpenVR inputs for Roblox.</returns>
    public TrackerInputList GetInputs()
    {
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
                headsetId = i;
            }
            else if (inputType == ETrackedDeviceClass.Controller || inputType == ETrackedDeviceClass.GenericTracker)
            {
                // TODO: Add properties from ETrackedDeviceProperty.
                trackerInputs.Add(new TrackerInput()
                {
                    DeviceId = i,
                    DeviceType = inputType,
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