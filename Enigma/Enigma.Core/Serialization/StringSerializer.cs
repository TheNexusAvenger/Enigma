using System.Numerics;
using System.Text;

namespace Enigma.Core.Serialization;

public class StringSerializer
{
    /// <summary>
    /// String builder for building the serialized data.
    /// </summary>
    private readonly StringBuilder _stringBuilder = new StringBuilder();

    /// <summary>
    /// Adds data to serialize.
    /// </summary>
    /// <param name="data">Data to serialize.</param>
    public void Add(object data)
    {
        // Add the separator if it isn't the first character.
        if (this._stringBuilder.Length != 0)
        {
            this._stringBuilder.Append('|');
        }
        
        // Add the string.
        this._stringBuilder.Append(data);
    }

    /// <summary>
    /// Adds a float to serialize with less precision.
    /// Only up to 3 decimal points are kept, with the rest ignored to save data.
    /// </summary>
    /// <param name="data">Float to serialize.</param>
    public void AddFloat(float data)
    {
        this.Add($"{data:f3}");
    }

    /// <summary>
    /// Adds a Vector3 (X,Y,Z) as individual floats.
    /// </summary>
    /// <param name="data">Vector3 to serialize.</param>
    public void AddVector3(Vector3 data)
    {
        this.AddFloat(data.X);
        this.AddFloat(data.Y);
        this.AddFloat(data.Z);
    }

    /// <summary>
    /// Adds a Quaternion (X,Y,Z,W) as individual floats.
    /// </summary>
    /// <param name="data">Quaternion to serialize.</param>
    public void AddQuaternion(Quaternion data)
    {
        this.AddFloat(data.X);
        this.AddFloat(data.Y);
        this.AddFloat(data.Z);
        this.AddFloat(data.W);
    }
    
    /// <summary>
    /// Returns the current data as a serialized string.
    /// </summary>
    /// <returns>Stored data as a string.</returns>
    public new string ToString()
    {
        return this._stringBuilder.ToString();
    }
}