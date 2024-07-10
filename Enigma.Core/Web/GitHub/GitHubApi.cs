using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Enigma.Core.Diagnostic;
using Enigma.Core.Shim.Http;
using Enigma.Core.Web.GitHub.Model;

namespace Enigma.Core.Web.GitHub;

public class GitHubApi
{
    /// <summary>
    /// Http client used to access GitHub.
    /// </summary>
    private readonly IHttpClient _httpClient;

    /// <summary>
    /// Creates a GitHub API utility.
    /// </summary>
    /// <param name="httpClient">HttpClient implementation to use.</param>
    public GitHubApi(IHttpClient httpClient)
    {
        this._httpClient = httpClient;
        this._httpClient.SetUserAgent("Enigma-Version-Check");
    }
    
    /// <summary>
    /// Creates a GitHub API utility.
    /// </summary>
    public GitHubApi() : this(new HttpClientWrapper())
    {
        
    }

    /// <summary>
    /// Returns the latest GitHub release for the project.
    /// </summary>
    /// <param name="projectVersionData">Version data of the project.</param>
    /// <returns>The latest GitHub release for the project, if one exists.</returns>
    public async Task<GitHubRelease?> GetLatestReleaseAsync(ProjectVersionData projectVersionData)
    {
        // Return if there is a null value.
        if (projectVersionData.GitHubUser == null)
        {
            Logger.Warn("ProjectVersionData.GitHubUser was not provided. No releases on GitHub can be checked.");
            return null;
        } 
        if (projectVersionData.GitHubProject == null)
        {
            Logger.Warn("ProjectVersionData.GitHubProject was not provided. No releases on GitHub can be checked.");
            return null;
        } 

        // Get the latest release from GitHub.
        try
        {
            // Get the latest releases of the project.
            var response = await this._httpClient.GetAsync($"https://api.github.com/repos/{projectVersionData.GitHubUser}/{projectVersionData.GitHubProject}/releases");
            if (!response.IsSuccessStatusCode)
            {
                Logger.Warn($"Received HTTP {response.StatusCode} ({response.ReasonPhrase}) from GitHub API. Update check can't be performed.");
                return null;
            }
            
            // Read the response and parse the JSON.
            var releasesResponse = await response.Content.ReadAsStringAsync();
            var releases = JsonSerializer.Deserialize<List<GitHubRelease>>(releasesResponse, GitHubReleaseJsonContext.Default.ListGitHubRelease);
            if (releases == null || releases.Count == 0)
            {
                Logger.Warn("No releases were returned for the GitHub project.");
                return null;
            }
            return releases[0];
        }
        catch (Exception exception)
        {
            Logger.Error($"An error occured trying to get the latest release information from GitHub.\n{exception}");
            return null;
        }
    }
}