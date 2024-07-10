using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Enigma.Core.Web.GitHub.Model;

public class GitHubRelease
{
    /// <summary>
    /// HTML URL to view the release.
    /// </summary>
    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }

    /// <summary>
    /// Name of the tag for the release.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

[JsonSerializable(typeof(GitHubRelease))]
[JsonSerializable(typeof(List<GitHubRelease>))]
[JsonSourceGenerationOptions(WriteIndented=true, IncludeFields = true)]
internal partial class GitHubReleaseJsonContext : JsonSerializerContext
{
}