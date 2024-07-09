using System.Collections.Generic;
using Enigma.Core.OpenVr;
using Enigma.Core.OpenVr.Model;
using NUnit.Framework;

namespace Enigma.Core.Test.OpenVr;

public class SteamVrSettingsStateTest
{
    private SteamVrSettingsState _steamVrSettingsState;

    [SetUp]
    public void SetUp()
    {
        this._steamVrSettingsState = new SteamVrSettingsState("");
    }
    
    [Test]
    public void TestGetTrackerRole()
    {
        Assert.That(SteamVrSettingsState.GetTrackerRole("TrackerRole_LeftShoulder"), Is.EqualTo(TrackerRole.LeftShoulder));
        Assert.That(SteamVrSettingsState.GetTrackerRole("TrackerRole_Other"), Is.Null);
    }

    [Test]
    public void ReloadSettings()
    {
        this._steamVrSettingsState.ReloadSettings(new SteamVrSettings()
        {
            Trackers = new Dictionary<string, string>()
            {
                {"/devices/lighthouse/12345", "TrackerRole_LeftShoulder"},
            }
        });
        Assert.That(this._steamVrSettingsState.GetRole("/devices/lighthouse/12345"), Is.EqualTo(TrackerRole.LeftShoulder));
        Assert.That(this._steamVrSettingsState.GetRole("/devices/lighthouse/23456"), Is.EqualTo(TrackerRole.None));
       
        this._steamVrSettingsState.ReloadSettings(new SteamVrSettings()
        {
            Trackers = new Dictionary<string, string>()
            {
                {"/devices/lighthouse/12345", "TrackerRole_RightShoulder"},
                {"/devices/lighthouse/23456", "TrackerRole_LeftKnee"},
                {"/devices/lighthouse/34567", "TrackerRole_Unknown"},
            }
        });
        Assert.That(this._steamVrSettingsState.GetRole("/devices/lighthouse/12345"), Is.EqualTo(TrackerRole.RightShoulder));
        Assert.That(this._steamVrSettingsState.GetRole("/devices/lighthouse/23456"), Is.EqualTo(TrackerRole.LeftKnee));
    }

    [Test]
    public void ReloadSettingsNullSteamVrSettings()
    {
        this._steamVrSettingsState.ReloadSettings(null);
        Assert.That(this._steamVrSettingsState.GetRole("/devices/lighthouse/12345"), Is.EqualTo(TrackerRole.None));
    }

    [Test]
    public void ReloadSettingsNullTrackers()
    {
        this._steamVrSettingsState.ReloadSettings(new SteamVrSettings());
        Assert.That(this._steamVrSettingsState.GetRole("/devices/lighthouse/12345\""), Is.EqualTo(TrackerRole.None));
    }
}