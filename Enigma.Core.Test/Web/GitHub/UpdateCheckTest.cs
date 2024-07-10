using Enigma.Core.Web.GitHub;
using Enigma.Core.Web.GitHub.Model;
using NUnit.Framework;

namespace Enigma.Core.Test.Web.GitHub;

public class UpdateCheckTest
{
    private UpdateCheck _updateCheck;
    private ProjectVersionData _projectVersionData;
    private GitHubRelease _latestGitHubRelease;

    [SetUp]
    public void SetUp()
    {
        this._updateCheck = new UpdateCheck();
        this._projectVersionData = new ProjectVersionData()
        {
            Version = "v1",
        };
        this._latestGitHubRelease = new GitHubRelease()
        {
            Name = "v2",
        };
    }

    [Test]
    public void TestCheckForUpdates()
    {
        Assert.That(this._updateCheck.CheckForUpdates(this._projectVersionData, this._latestGitHubRelease), Is.EqualTo(this._latestGitHubRelease));
    }

    [Test]
    public void TestCheckForUpdatesNullProjectData()
    {
        Assert.That(this._updateCheck.CheckForUpdates(null, this._latestGitHubRelease), Is.Null);
    }

    [Test]
    public void TestCheckForUpdatesNullProjectDataVersion()
    {
        this._projectVersionData.Version = null;
        Assert.That(this._updateCheck.CheckForUpdates(this._projectVersionData, this._latestGitHubRelease), Is.Null);
    }

    [Test]
    public void TestCheckForUpdatesNullRelease()
    {
        Assert.That(this._updateCheck.CheckForUpdates(this._projectVersionData, null), Is.Null);
    }

    [Test]
    public void TestCheckForUpdatesNullReleaseName()
    {
        this._latestGitHubRelease.Name = null;
        Assert.That(this._updateCheck.CheckForUpdates(this._projectVersionData, this._latestGitHubRelease), Is.Null);
    }

    [Test]
    public void TestCheckForUpdatesSameVersion()
    {
        this._latestGitHubRelease.Name = "v1";
        Assert.That(this._updateCheck.CheckForUpdates(this._projectVersionData, this._latestGitHubRelease), Is.Null);
    }
}