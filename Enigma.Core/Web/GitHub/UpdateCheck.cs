using System.Threading.Tasks;
using Enigma.Core.Diagnostic;
using Enigma.Core.Web.GitHub.Model;

namespace Enigma.Core.Web.GitHub;

public class UpdateCheck
{
    /// <summary>
    /// GitHub API utility to read releases from.
    /// </summary>
    private readonly GitHubApi _gitHubApi = new GitHubApi();
    
    /// <summary>
    /// Checks if an update is available.
    /// </summary>
    /// <param name="projectVersionData">Project version data stored in the application.</param>
    /// <param name="latestGitHubRelease">Release from GitHub to check against.</param>
    /// <returns>GitHub release to update to if there is an update.</returns>
    public GitHubRelease? CheckForUpdates(ProjectVersionData? projectVersionData, GitHubRelease? latestGitHubRelease)
    {
        // Return false if the project version or release were not provided.
        if (projectVersionData?.Version == null)
        {
            Logger.Debug("Project version information was not provided. Unable to check for updates.");
            return null;
        }
        if (latestGitHubRelease?.Name == null)
        {
            Logger.Debug("GitHub release information was not provided. Unable to check for updates.");
            return null;
        }
        
        // Check if the versions match.
        if (projectVersionData.Version == latestGitHubRelease.Name)
        {
            Logger.Debug($"Enigma is up to date ({projectVersionData.Version})");
            return null;
        }
        Logger.Info($"A new release of Enigma is available ({projectVersionData.Version} -> {latestGitHubRelease.Name}).\nDownload: {latestGitHubRelease.HtmlUrl}");
        return latestGitHubRelease;
    }

    /// <summary>
    /// Checks if an update is available.
    /// </summary>
    /// <returns>Whether an update is available.</returns>
    public async Task<GitHubRelease?> CheckForUpdatesAsync()
    {
        var projectVersionData = await ProjectVersionData.GetVersionDataAsync();
        return this.CheckForUpdates(projectVersionData, await this._gitHubApi.GetLatestReleaseAsync(projectVersionData));
    }
}