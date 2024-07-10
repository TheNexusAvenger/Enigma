using System.Net;
using Enigma.Core.Test.TestShim;
using Enigma.Core.Web.GitHub;
using Enigma.Core.Web.GitHub.Model;
using NUnit.Framework;

namespace Enigma.Core.Test.Web.GitHub;

public class GitHubApiTest
{
    private TestHttpClient _httpClient;
    
    private GitHubApi _gitHubApi;

    [SetUp]
    public void SetUp()
    {
        this._httpClient = new TestHttpClient();
        this._gitHubApi = new GitHubApi(_httpClient);
    }

    [Test]
    public void TestGetLatestReleaseAsync()
    {
        this._httpClient.SetResponse("https://api.github.com/repos/Owner/Project/releases", HttpStatusCode.OK, "[{\"name\":\"v2\",\"html_url\":\"testUrl2\"}, {\"name\":\"v1\",\"html_url\":\"testUrl1\"}]");

        var release = this._gitHubApi.GetLatestReleaseAsync(new ProjectVersionData()
        {
            GitHubUser = "Owner",
            GitHubProject = "Project",
        }).Result;
        Assert.That(release!.HtmlUrl, Is.EqualTo("testUrl2"));
        Assert.That(release!.Name, Is.EqualTo("v2"));
    }

    [Test]
    public void TestGetLatestReleaseAsyncNullProjectVersion()
    {
        var release = this._gitHubApi.GetLatestReleaseAsync(null).Result;
        Assert.That(release, Is.Null);
    }

    [Test]
    public void TestGetLatestReleaseAsyncNullUser()
    {
        var release = this._gitHubApi.GetLatestReleaseAsync(new ProjectVersionData()
        {
            GitHubProject = "Project",
        }).Result;
        Assert.That(release, Is.Null);
    }

    [Test]
    public void TestGetLatestReleaseAsyncNullProject()
    {
        var release = this._gitHubApi.GetLatestReleaseAsync(new ProjectVersionData()
        {
            GitHubUser = "Owner",
        }).Result;
        Assert.That(release, Is.Null);
    }

    [Test]
    public void TestGetLatestReleaseAsyncException()
    {
        // When SetResponse is not called, an exception is thrown.
        var release = this._gitHubApi.GetLatestReleaseAsync(new ProjectVersionData()
        {
            GitHubUser = "Owner",
            GitHubProject = "Project",
        }).Result;
        Assert.That(release, Is.Null);
    }

    [Test]
    public void TestGetLatestReleaseBadHttpStatus()
    {
        this._httpClient.SetResponse("https://api.github.com/repos/Owner/Project/releases", HttpStatusCode.Forbidden, "Forbidden");
        
        var release = this._gitHubApi.GetLatestReleaseAsync(new ProjectVersionData()
        {
            GitHubUser = "Owner",
            GitHubProject = "Project",
        }).Result;
        Assert.That(release, Is.Null);
    }

    [Test]
    public void TestGetLatestReleaseAsyncNoReleases()
    {
        this._httpClient.SetResponse("https://api.github.com/repos/Owner/Project/releases", HttpStatusCode.OK, "[]");

        var release = this._gitHubApi.GetLatestReleaseAsync(new ProjectVersionData()
        {
            GitHubUser = "Owner",
            GitHubProject = "Project",
        }).Result;
        Assert.That(release, Is.Null);
    }

    [Test]
    public void TestGetLatestReleaseAsyncMalformedReleases()
    {
        this._httpClient.SetResponse("https://api.github.com/repos/Owner/Project/releases", HttpStatusCode.OK, "notJson");

        var release = this._gitHubApi.GetLatestReleaseAsync(new ProjectVersionData()
        {
            GitHubUser = "Owner",
            GitHubProject = "Project",
        }).Result;
        Assert.That(release, Is.Null);
    }
}