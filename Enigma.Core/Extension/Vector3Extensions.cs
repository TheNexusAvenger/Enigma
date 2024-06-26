using System.Numerics;

namespace Enigma.Core.Extension;

public static class Vector3Extensions
{
    /// <summary>
    /// Multiplier to convert from meters to feet.
    /// </summary>
    public const float MetersToFeetMultiplier = 3.28084f;
    
    /// <summary>
    /// Converts a Vector3 in meters to feet.
    /// </summary>
    /// <param name="this">Vector3 to convert.</param>
    /// <returns>Vector3 scaled up to feet.</returns>
    public static Vector3 ConvertMetersToFeet(this Vector3 @this)
    {
        return new Vector3(@this.X * MetersToFeetMultiplier, @this.Y * MetersToFeetMultiplier, @this.Z * MetersToFeetMultiplier);
    }
}