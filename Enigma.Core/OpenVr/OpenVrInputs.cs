using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Enigma.Core.Diagnostic;
using Enigma.Core.Extension;
using Enigma.Core.OpenVr.Model;
using Enigma.Core.Shim.OpenVr;
using Microsoft.Extensions.Logging;
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
    private IOVRSystem? _ovrSystem;

    /// <summary>
    /// Creates a OpenVR inputs reader.
    /// </summary>
    /// <param name="steamVrSettingsState">State of the SteamVR settings to read tracker roles.</param>
    /// <param name="ovrSystem">Optional IOVRSystem implementation to used.</param>
    public OpenVrInputs(SteamVrSettingsState steamVrSettingsState, IOVRSystem? ovrSystem = null)
    {
        this._steamVrSettingsState = steamVrSettingsState;
        this._ovrSystem = ovrSystem;
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
                this._ovrSystem = new OVRSystem(new Application(Application.ApplicationType.Background).OVRSystem);
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
            catch (DllNotFoundException)
            {
                Logger.Error("The file openvr_api.dll is missing. This file is required for Enigma to start.");
                throw;
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
    public string? GetTrackerStringProperty(uint trackerIndex, ETrackedDeviceProperty property)
    {
        // Throw an exception if OpenVR is not initialized.
        if (this._ovrSystem == null)
        {
            throw new InvalidOperationException("OpenVR is not initialized.");
        }
        
        // Build and return the message if it was a success.
        var error = ETrackedPropertyError.TrackedProp_Success;
        var propertyBuilder = new StringBuilder(128);
        this._ovrSystem.GetStringTrackedDeviceProperty(trackerIndex, property, propertyBuilder, (uint) propertyBuilder.Capacity, ref error);
        return error == ETrackedPropertyError.TrackedProp_Success ? propertyBuilder.ToString() : null;
    }

    /// <summary>
    /// Returns the hardware id for a tracker.
    /// Placeholder strings will be used when a property is not provided.
    /// </summary>
    /// <param name="trackerIndex">Index of the tracker to get the id for.</param>
    /// <returns>Hardware id of the track.</returns>
    public string GetHardwareId(uint trackerIndex)
    {
        // Return based on Prop_RegisteredDeviceType (Vive trackers, Amethyst).
        var registeredDeviceType = this.GetTrackerStringProperty(trackerIndex, ETrackedDeviceProperty.Prop_RegisteredDeviceType_String);
        if (registeredDeviceType != null)
        {
            return $"/devices/{registeredDeviceType}";
        }
        
        // Return based on TrackingSystemName and SerialNumber (Tundra trackers).
        var trackingSystem = this.GetTrackerStringProperty(trackerIndex, ETrackedDeviceProperty.Prop_TrackingSystemName_String) ?? "<UNDEFINED_TrackingSystemName>";
        var serialNumber = this.GetTrackerStringProperty(trackerIndex, ETrackedDeviceProperty.Prop_SerialNumber_String) ?? "<UNDEFINED_SerialNumber>";
        return $"/devices/{trackingSystem}/{serialNumber}";
    }

    /// <summary>
    /// Attempts to guess the tracker role for a tracker.
    /// </summary>
    /// <param name="trackerIndex">Device index of the tracker to guess the role of.</param>
    /// <returns>Guessed tracker role for the tracker.</returns>
    public TrackerRole GuessTrackerRole(uint trackerIndex)
    {
        // Iterate over the tracker roles to try to find role.
        var controllerType = this.GetTrackerStringProperty(trackerIndex, ETrackedDeviceProperty.Prop_ControllerType_String);
        var controllerTypeNoSpaces = (controllerType ?? "").Replace("_", "");
        var serialNumber = this.GetTrackerStringProperty(trackerIndex, ETrackedDeviceProperty.Prop_SerialNumber_String);
        var serialNumberNoSpaces = (serialNumber ?? "").Replace(" ", "");
        foreach (TrackerRole trackerRole in Enum.GetValues(typeof(TrackerRole)))
        {
            // Return based on the controller type (Vive trackers, Tundra trackers, Amethyst).
            var trackerRoleName = trackerRole.ToString();
            if (controllerTypeNoSpaces.EndsWith(trackerRoleName, StringComparison.InvariantCultureIgnoreCase))
            {
                return trackerRole;
            }
            
            // Return based on the serial number (Standable).
            if (serialNumberNoSpaces.EndsWith(trackerRoleName, StringComparison.InvariantCultureIgnoreCase))
            {
                return trackerRole;
            }
        }
        
        // Warn if the tracker role can't be guessed.
        if (controllerType != null && controllerType != "vive_tracker")
        {
            Logger.LogOnce(LogLevel.Warning, $"Device {this.GetHardwareId(trackerIndex)} tracker role could not be guessed.\nPlease create a GitHub Issue with the contents of `list-devices --masked`.");
        }
        
        // Return no role (can't determine role).
        return TrackerRole.None;
    }

    /// <summary>
    /// Lists the connected inputs in OpenVR, including non-trackers.
    /// </summary>
    /// <returns>List of OpenVR devices.</returns>
    public List<OpenVrDevice> ListDevices()
    {
        // Throw an exception if OpenVR is not initialized.
        if (this._ovrSystem == null)
        {
            throw new InvalidOperationException("OpenVR is not initialized.");
        }
        
        // List the devices.
        var devices = new List<OpenVrDevice>();
        for (uint i = 0; i < DevicesToCheck; i++)
        {
            // Ignore the device if it is invalid.
            var deviceType = this._ovrSystem.GetTrackedDeviceClass(i);
            if (deviceType == ETrackedDeviceClass.Invalid) continue;
            
            // Add the device.
            var hardwareId = this.GetHardwareId(i);
            var device = new OpenVrDevice()
            {
                DeviceId = i,
                HardwareId = this.GetHardwareId(i),
                DeviceType = deviceType,
            };
            if (device.DeviceType == ETrackedDeviceClass.GenericTracker)
            {
                device.GuessedRole = this.GuessTrackerRole(i);
                device.SteamVrRole = this._steamVrSettingsState.GetRole(hardwareId);
            }
            foreach (ETrackedDeviceProperty deviceProperty in Enum.GetValues(typeof(ETrackedDeviceProperty)))
            {
                if (!deviceProperty.ToString().EndsWith("_String")) continue;
                var devicePropertyValue = this.GetTrackerStringProperty(i, deviceProperty);
                if (string.IsNullOrEmpty(devicePropertyValue)) continue;
                device.StringProperties[deviceProperty] = devicePropertyValue;
            }
            devices.Add(device);
        }
        return devices;
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
        var trackerInputs = new TrackerInputList();
        for (uint i = 0; i < DevicesToCheck; i++)
        {
            // Return if the device is not at tracker.
            var inputType = this._ovrSystem.GetTrackedDeviceClass(i);
            if (inputType != ETrackedDeviceClass.GenericTracker) continue;
            
            // Get the tracker information.
            if (!this._ovrSystem.IsTrackedDeviceConnected(i)) continue;
            var hardwareId = this.GetHardwareId(i);
            var steamVrTrackerRole = this._steamVrSettingsState.GetRole(hardwareId);
            
            // Add the tracker information.
            this.GuessTrackerRole(i);
            trackerInputs.Add(new TrackerInput()
            {
                DeviceId = i,
                DeviceType = inputType,
                TrackerRole = (steamVrTrackerRole != TrackerRole.None ? steamVrTrackerRole: this.GuessTrackerRole(i)),
            });
        }
        
        // Add the relative tracker positions.
        foreach (var input in trackerInputs)
        {
            var trackerPoseData = poseArray[input.DeviceId];
            var trackerPose = trackerPoseData.mDeviceToAbsoluteTracking.ToMatrix4x4();
            var trackerRotation = Quaternion.CreateFromRotationMatrix(trackerPose).FlipHandedness();
            var trackerVelocity = new Vector3(trackerPoseData.vVelocity.v0, trackerPoseData.vVelocity.v1, trackerPoseData.vVelocity.v2);
            input.Rotation = trackerRotation;
            input.Position = trackerPose.Translation.ConvertMetersToFeet();
            input.Velocity = trackerVelocity.ConvertMetersToFeet();
        }
        
        // Return the inputs.
        return trackerInputs;
    }
}