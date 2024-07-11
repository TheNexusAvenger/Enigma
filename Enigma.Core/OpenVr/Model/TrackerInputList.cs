using System.Collections.Generic;
using Enigma.Core.Serialization;

namespace Enigma.Core.OpenVr.Model;

public class TrackerInputList : List<TrackerInput>
{
    /// <summary>
    /// API version of the tracker input list.
    /// </summary>
    public const int ApiVersion = 2;
    
    /// <summary>
    /// Serializes the tracker input list.
    /// </summary>
    /// <returns>Serialized tracker input data.</returns>
    public string Serialize()
    {
        // Store the API version and total inputs.
        var serializer = new StringSerializer();
        serializer.Add(ApiVersion);
        serializer.Add(this.Count);
        
        // Store the inputs.
        foreach (var input in this)
        {
            serializer.Add(4); // 4 properties are sent.
            serializer.AddDeviceProperty(DeviceProperty.TrackerRole, 1);
            serializer.Add((int) input.TrackerRole);
            serializer.AddDeviceProperty(DeviceProperty.Position, 3);
            serializer.AddVector3(input.Position);
            serializer.AddDeviceProperty(DeviceProperty.Rotation, 4);
            serializer.AddQuaternion(input.Rotation);
            serializer.AddDeviceProperty(DeviceProperty.Velocity, 3);
            serializer.AddVector3(input.Velocity);
        }
        
        // Return the serialized string.
        return serializer.ToString();
    }
}