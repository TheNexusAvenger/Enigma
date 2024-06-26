using System.Numerics;

namespace Enigma.Core.Extension;

public static class QuaternionExtensions
{
    /// <summary>
    /// Flips the "handedness" of a Quaternion.
    /// Flipping is required for use on Roblox.
    /// </summary>
    /// <param name="this">Quaternion to change.</param>
    /// <returns>Quaternion with the flipped handedness.</returns>
    public static Quaternion FlipHandedness(this Quaternion @this)
    {
        return new Quaternion(-@this.X, -@this.Y, -@this.Z, @this.W);
    }
}