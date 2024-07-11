using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Enigma.Core.OpenVr.Model;
using Valve.VR;

namespace Enigma.Core.Diagnostic;

public partial class OpenVrPropertyMasker
{
    /// <summary>
    /// Mask character to used.
    /// </summary>
    public const char MaskCharacter = '#';

    /// <summary>
    /// Mask methods for specific properties.
    /// </summary>
    private static readonly Dictionary<ETrackedDeviceProperty, Func<string, string>> MaskMethods = new Dictionary<ETrackedDeviceProperty, Func<string, string>>()
    {
        {ETrackedDeviceProperty.Prop_SerialNumber_String, MaskDeviceId},
        {ETrackedDeviceProperty.Prop_TrackingFirmwareVersion_String, MaskString},
        {ETrackedDeviceProperty.Prop_HardwareRevision_String, MaskString},
        {ETrackedDeviceProperty.Prop_AllWirelessDongleDescriptions_String, MaskString},
        {ETrackedDeviceProperty.Prop_ConnectedWirelessDongle_String, MaskString},
        {ETrackedDeviceProperty.Prop_Firmware_ProgrammingTarget_String, MaskDeviceId},
        {ETrackedDeviceProperty.Prop_CompositeFirmwareVersion_String, MaskString},
        {ETrackedDeviceProperty.Prop_RegisteredDeviceType_String, MaskDeviceId},
        {ETrackedDeviceProperty.Prop_ManufacturerSerialNumber_String, MaskString},
        {ETrackedDeviceProperty.Prop_ComputedSerialNumber_String, MaskString},
        {ETrackedDeviceProperty.Prop_CameraFirmwareDescription_String, MaskString},
        {ETrackedDeviceProperty.Prop_HmdColumnCorrectionSettingPrefix_String, MaskString},
        {ETrackedDeviceProperty.Prop_Audio_DefaultPlaybackDeviceId_String, MaskString},
        {ETrackedDeviceProperty.Prop_Audio_DefaultRecordingDeviceId_String, MaskString},
    };

    /// <summary>
    /// Compile-time regular expression for SteamVR lighthouse tracker ids.
    /// </summary>
    [GeneratedRegex("(LH[A-Z]\\-)([A-Z0-9])([A-Z0-9]+)([A-Z0-9])$")]
    private static partial Regex LighthouseDeviceIdRegex();

    /// <summary>
    /// Masks a string.
    /// Except for tracker roles, all but the first 2 and last 2 characters will be masked.
    /// </summary>
    /// <param name="data">String to mask.</param>
    /// <returns>Masked version of a string.</returns>
    public static string MaskString(string data)
    {
        // Return if the string matches a role.
        foreach (TrackerRole trackerRole in Enum.GetValues(typeof(TrackerRole)))
        {
            if (!data.Replace(" ", "").Equals(trackerRole.ToString(), StringComparison.InvariantCultureIgnoreCase)) continue;
            return data;
        }
        
        // Return if the string is too short.
        if (data.Length <= 4)
        {
            return new string(MaskCharacter, data.Length);
        }

        // Mask all but the first and last character.
        return $"{data.Substring(0, 2)}{new string(MaskCharacter, data.Length - 4)}{data.Substring(data.Length - 2, 2)}";
    }

    /// <summary>
    /// Masks a potential device id.
    /// </summary>
    /// <param name="data">Potential device id to mask.</param>
    /// <returns>Masked version of the device id.</returns>
    public static string MaskDeviceId(string data)
    {
        return LighthouseDeviceIdRegex().Replace(data, match => $"{match.Groups[1]}{match.Groups[2]}{new string(MaskCharacter, match.Groups[3].Length)}{match.Groups[4]}");
    }

    /// <summary>
    /// Masks an OpenVR property.
    /// </summary>
    /// <param name="property">Property to mask.</param>
    /// <param name="data">Value of the property to mask.</param>
    /// <returns>Masked value of the property.</returns>
    public static string MaskProperty(ETrackedDeviceProperty property, string data)
    {
        return !MaskMethods.TryGetValue(property, out var maskMethod) ? data : maskMethod.Invoke(data);
    }
}