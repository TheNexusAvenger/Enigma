using System.Numerics;
using Enigma.Core.Serialization;
using NUnit.Framework;

namespace Enigma.Core.Test.Serialization;

public class StringSerializerTest
{
    [Test]
    public void TestAddSingleEntry()
    {
        var stringSerializer = new StringSerializer();
        stringSerializer.Add("Test1");
        Assert.That(stringSerializer.ToString(), Is.EqualTo("Test1"));
    }
    
    [Test]
    public void TestAddMultipleEntries()
    {
        var stringSerializer = new StringSerializer();
        stringSerializer.Add("Test1");
        stringSerializer.Add("Test2");
        stringSerializer.Add("Test3");
        Assert.That(stringSerializer.ToString(), Is.EqualTo("Test1|Test2|Test3"));
    }
    
    [Test]
    public void TestAddFloat()
    {
        var stringSerializer = new StringSerializer();
        stringSerializer.AddFloat(123);
        stringSerializer.AddFloat(123.123f);
        stringSerializer.AddFloat(123.123456f);
        Assert.That(stringSerializer.ToString(), Is.EqualTo("123.000|123.123|123.123"));
    }

    [Test]
    public void TestAddVector3()
    {
        var stringSerializer = new StringSerializer();
        stringSerializer.AddVector3(new Vector3(1, 2, 3));
        Assert.That(stringSerializer.ToString(), Is.EqualTo("1.000|2.000|3.000"));
    }

    [Test]
    public void TestAddQuaternion()
    {
        var stringSerializer = new StringSerializer();
        stringSerializer.AddQuaternion(new Quaternion(1, 2, 3, 4));
        Assert.That(stringSerializer.ToString(), Is.EqualTo("1.000|2.000|3.000|4.000"));
    }
}