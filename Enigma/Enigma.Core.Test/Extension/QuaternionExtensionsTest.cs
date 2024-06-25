using System.Numerics;
using Enigma.Core.Extension;
using NUnit.Framework;

namespace Enigma.Core.Test.Extension;

public class QuaternionExtensionsTest
{
    [Test]
    public void TestFlipHandedness()
    {
        Assert.That(new Quaternion(0.1f, 0.2f, 0.3f, 0.4f).FlipHandedness(), Is.EqualTo(new Quaternion(-0.1f, -0.2f, -0.3f, 0.4f)));
    }
}