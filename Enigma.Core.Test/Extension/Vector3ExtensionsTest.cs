using System.Numerics;
using Enigma.Core.Extension;
using NUnit.Framework;

namespace Enigma.Core.Test.Extension;

public class Vector3ExtensionsTest
{
    [Test]
    public void TestConvertMetersToFeet()
    {
        Assert.That(new Vector3(2, 3, 4).ConvertMetersToFeet(), Is.EqualTo(new Vector3(2 * 3.28084f, 3 * 3.28084f, 4 * 3.28084f)));
    }
}