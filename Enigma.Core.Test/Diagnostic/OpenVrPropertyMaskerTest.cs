using Enigma.Core.Diagnostic;
using NUnit.Framework;
using Valve.VR;

namespace Enigma.Core.Test.Diagnostic;

public class OpenVrPropertyMaskerTest
{
    [Test]
    public void TestMaskString()
    {
        Assert.That(OpenVrPropertyMasker.MaskString("12345"), Is.EqualTo("12#45"));
        Assert.That(OpenVrPropertyMasker.MaskString("123456789"), Is.EqualTo("12#####89"));
    }
    
    [Test]
    public void TestMaskStringRole()
    {
        Assert.That(OpenVrPropertyMasker.MaskString("Left Shoulder"), Is.EqualTo("Left Shoulder"));
        Assert.That(OpenVrPropertyMasker.MaskString("leftshoulder"), Is.EqualTo("leftshoulder"));
    }
    
    [Test]
    public void TestMaskStringShort()
    {
        Assert.That(OpenVrPropertyMasker.MaskString(""), Is.EqualTo(""));
        Assert.That(OpenVrPropertyMasker.MaskString("1"), Is.EqualTo("#"));
        Assert.That(OpenVrPropertyMasker.MaskString("12"), Is.EqualTo("##"));
        Assert.That(OpenVrPropertyMasker.MaskString("123"), Is.EqualTo("###"));
        Assert.That(OpenVrPropertyMasker.MaskString("1234"), Is.EqualTo("####"));
    }
    
    [Test]
    public void TestMaskDeviceIdLighthouseDevice()
    {
        Assert.That(OpenVrPropertyMasker.MaskDeviceId("LHR-12ABCD78"), Is.EqualTo("LHR-1######8"));
        Assert.That(OpenVrPropertyMasker.MaskDeviceId("LHB-12ABCD78"), Is.EqualTo("LHB-1######8"));
    }
    
    [Test]
    public void TestMaskDeviceIdOculusDevice()
    {
        Assert.That(OpenVrPropertyMasker.MaskDeviceId("Some123Id"), Is.EqualTo("So#####Id"));
        Assert.That(OpenVrPropertyMasker.MaskDeviceId("oculus/Some123Id"), Is.EqualTo("oculus/So#####Id"));
        Assert.That(OpenVrPropertyMasker.MaskDeviceId("/devices/12345/oculus/Some123Id"), Is.EqualTo("/devices/12#45/oculus/So#####Id"));
    }
    
    [Test]
    public void TestMaskDeviceIdRegisteredLighthouseDevice()
    {
        Assert.That(OpenVrPropertyMasker.MaskDeviceId("htc/vive_trackerLHR-12ABCD78"), Is.EqualTo("htc/vive_trackerLHR-1######8"));
        Assert.That(OpenVrPropertyMasker.MaskDeviceId("htc/vive_trackerLHB-12ABCD78"), Is.EqualTo("htc/vive_trackerLHB-1######8"));
    }
    
    [Test]
    public void TestMaskDeviceIdOtherString()
    {
        Assert.That(OpenVrPropertyMasker.MaskDeviceId("Left Shoulder"), Is.EqualTo("Left Shoulder"));
        Assert.That(OpenVrPropertyMasker.MaskDeviceId("amethyst/vr_tracker/AME-WAIST"), Is.EqualTo("amethyst/vr_tracker/AME-WAIST"));
    }

    [Test]
    public void TestMaskProperty()
    {
        Assert.That(OpenVrPropertyMasker.MaskProperty(ETrackedDeviceProperty.Prop_TrackingFirmwareVersion_String, "htc/vive_trackerLHR-12ABCD78"), Is.EqualTo("ht########################78"));
        Assert.That(OpenVrPropertyMasker.MaskProperty(ETrackedDeviceProperty.Prop_RegisteredDeviceType_String, "htc/vive_trackerLHR-12ABCD78"), Is.EqualTo("htc/vive_trackerLHR-1######8"));
    }

    [Test]
    public void TestMaskPropertyNoMaskMethod()
    {
        Assert.That(OpenVrPropertyMasker.MaskProperty(ETrackedDeviceProperty.Prop_ControllerType_String, "Test"), Is.EqualTo("Test"));
    }
}