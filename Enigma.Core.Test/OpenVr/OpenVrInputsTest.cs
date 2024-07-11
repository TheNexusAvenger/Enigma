using System;
using System.Collections.Generic;
using System.Numerics;
using Enigma.Core.Extension;
using Enigma.Core.OpenVr;
using Enigma.Core.OpenVr.Model;
using Enigma.Core.Test.TestShim;
using NUnit.Framework;
using Valve.VR;

namespace Enigma.Core.Test.OpenVr;

public class OpenVrInputsTest
{
    private SteamVrSettingsState _steamVrSettingsState;
    
    private TestOVRSystem _ovrSystem;

    private OpenVrInputs _openVrInputs;

    [SetUp]
    public void SetUp()
    {
        this._ovrSystem = new TestOVRSystem();
        this._ovrSystem.Devices.Add(new TestOpenVrDevice()
        {
            DeviceClass = ETrackedDeviceClass.HMD,
            Properties = new Dictionary<ETrackedDeviceProperty, string>()
            {
                { ETrackedDeviceProperty.Prop_RegisteredDeviceType_String, "test/headset" },
            },
        });
        this._steamVrSettingsState = new SteamVrSettingsState("");
        this._openVrInputs = new OpenVrInputs(_steamVrSettingsState, this._ovrSystem);
    }

    [Test]
    public void TestGetTrackerStringProperty()
    {
        this._ovrSystem.Devices.Add(new TestOpenVrDevice()
        {
            DeviceClass = ETrackedDeviceClass.GenericTracker,
            Properties = new Dictionary<ETrackedDeviceProperty, string>()
            {
                { ETrackedDeviceProperty.Prop_ManufacturerName_String, "Valve" },
            },
        });
        
        Assert.That(this._openVrInputs.GetTrackerStringProperty(1, ETrackedDeviceProperty.Prop_ManufacturerName_String), Is.EqualTo("Valve"));
    }

    [Test]
    public void TestGetTrackerStringPropertyUninitialized()
    {
        this._openVrInputs = new OpenVrInputs(_steamVrSettingsState);
        Assert.Throws<InvalidOperationException>(() =>
        {
            this._openVrInputs.GetTrackerStringProperty(1, ETrackedDeviceProperty.Prop_ManufacturerName_String);
        });
    }

    [Test]
    public void TestGetTrackerStringPropertyNonSuccess()
    {
        this._ovrSystem.Devices.Add(new TestOpenVrDevice()
        {
            DeviceClass = ETrackedDeviceClass.GenericTracker,
        });
        
        Assert.That(this._openVrInputs.GetTrackerStringProperty(1, ETrackedDeviceProperty.Prop_ManufacturerName_String), Is.Null);
    }

    [Test]
    public void TestGetHardwareIdRegisteredDeviceType()
    {
        this._ovrSystem.Devices.Add(new TestOpenVrDevice()
        {
            DeviceClass = ETrackedDeviceClass.GenericTracker,
            Properties = new Dictionary<ETrackedDeviceProperty, string>()
            {
                { ETrackedDeviceProperty.Prop_RegisteredDeviceType_String, "htc/vive_trackerLHR-12ABCD78" },
            },
        });
        
        Assert.That(this._openVrInputs.GetHardwareId(1), Is.EqualTo("/devices/htc/vive_trackerLHR-12ABCD78"));
    }

    [Test]
    public void TestGetHardwareIdDefault()
    {
        this._ovrSystem.Devices.Add(new TestOpenVrDevice()
        {
            DeviceClass = ETrackedDeviceClass.GenericTracker,
            Properties = new Dictionary<ETrackedDeviceProperty, string>()
            {
                { ETrackedDeviceProperty.Prop_TrackingSystemName_String, "lighthouse" },
                { ETrackedDeviceProperty.Prop_SerialNumber_String, "LHR-12ABCD78" },
            },
        });
        
        Assert.That(this._openVrInputs.GetHardwareId(1), Is.EqualTo("/devices/lighthouse/LHR-12ABCD78"));
    }

    [Test]
    public void TestGetHardwareIdMissingProperties()
    {
        this._ovrSystem.Devices.Add(new TestOpenVrDevice()
        {
            DeviceClass = ETrackedDeviceClass.GenericTracker,
        });
        
        Assert.That(this._openVrInputs.GetHardwareId(1), Is.EqualTo("/devices/<UNDEFINED_TrackingSystemName>/<UNDEFINED_SerialNumber>"));
    }
    
    [Test]
    public void TestGetListDevices()
    {
        this._ovrSystem.Devices.Add(new TestOpenVrDevice()
        {
            DeviceClass = ETrackedDeviceClass.GenericTracker,
            Properties = new Dictionary<ETrackedDeviceProperty, string>()
            {
                { ETrackedDeviceProperty.Prop_NumCameras_Int32, "12345" },
                { ETrackedDeviceProperty.Prop_TrackingSystemName_String, "lighthouse" },
                { ETrackedDeviceProperty.Prop_SerialNumber_String, "LHR-12ABCD78" },
            },
        });

        var devices = this._openVrInputs.ListDevices();
        Assert.That(devices.Count, Is.EqualTo(2));

        var device1 = devices[0];
        Assert.That(device1.DeviceId, Is.EqualTo(0));
        Assert.That(device1.HardwareId, Is.EqualTo("/devices/test/headset"));
        Assert.That(device1.DeviceType, Is.EqualTo(ETrackedDeviceClass.HMD));
        Assert.That(device1.StringProperties.Count, Is.EqualTo(1));
        Assert.That(device1.StringProperties[ETrackedDeviceProperty.Prop_RegisteredDeviceType_String], Is.EqualTo("test/headset"));
       
        var device2 = devices[1];
        Assert.That(device2.DeviceId, Is.EqualTo(1));
        Assert.That(device2.HardwareId, Is.EqualTo("/devices/lighthouse/LHR-12ABCD78"));
        Assert.That(device2.DeviceType, Is.EqualTo(ETrackedDeviceClass.GenericTracker));
        Assert.That(device2.StringProperties.Count, Is.EqualTo(2));
        Assert.That(device2.StringProperties[ETrackedDeviceProperty.Prop_TrackingSystemName_String], Is.EqualTo("lighthouse"));
        Assert.That(device2.StringProperties[ETrackedDeviceProperty.Prop_SerialNumber_String], Is.EqualTo("LHR-12ABCD78"));
    }

    [Test]
    public void TestGetListDevicesUninitialized()
    {
        this._openVrInputs = new OpenVrInputs(_steamVrSettingsState);
        Assert.Throws<InvalidOperationException>(() =>
        {
            this._openVrInputs.ListDevices();
        });
    }
    
    [Test]
    public void TestGetInputs()
    {
        this._ovrSystem.Devices.Add(new TestOpenVrDevice()
        {
            DeviceClass = ETrackedDeviceClass.GenericTracker,
            Properties = new Dictionary<ETrackedDeviceProperty, string>()
            {
                { ETrackedDeviceProperty.Prop_TrackingSystemName_String, "lighthouse" },
                { ETrackedDeviceProperty.Prop_SerialNumber_String, "LHR-12ABCD78" },
            },
        });
        this._ovrSystem.Devices.Add(new TestOpenVrDevice()
        {
            DeviceClass = ETrackedDeviceClass.GenericTracker,
            Connected = false,
            Properties = new Dictionary<ETrackedDeviceProperty, string>()
            {
                { ETrackedDeviceProperty.Prop_TrackingSystemName_String, "lighthouse" },
                { ETrackedDeviceProperty.Prop_SerialNumber_String, "LHR-12ABCD79" },
            },
        });

        var inputs = this._openVrInputs.GetInputs();
        Assert.That(inputs.Count, Is.EqualTo(1));

        var device1 = inputs[0];
        Assert.That(device1.DeviceId, Is.EqualTo(1));
        Assert.That(device1.DeviceType, Is.EqualTo(ETrackedDeviceClass.GenericTracker));
        Assert.That(device1.TrackerRole, Is.EqualTo(TrackerRole.None));
        Assert.That(device1.Position, Is.EqualTo(Vector3.Zero));
        Assert.That(device1.Rotation, Is.EqualTo(new Quaternion(-0.5f, 0, 0, 0)));
        Assert.That(device1.Velocity, Is.EqualTo(new Vector3(1, 2, 3).ConvertMetersToFeet()));
    }

    [Test]
    public void TestGetInputsUninitialized()
    {
        this._openVrInputs = new OpenVrInputs(_steamVrSettingsState);
        Assert.Throws<InvalidOperationException>(() =>
        {
            this._openVrInputs.GetInputs();
        });
    }
}