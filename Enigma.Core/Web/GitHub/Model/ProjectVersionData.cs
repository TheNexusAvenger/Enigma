using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Enigma.Core.Web.GitHub.Model;

public class ProjectVersionData
{
    /// <summary>
    /// Version of the application.
    /// </summary>
    public string? Version { get; set; }
    
    /// <summary>
    /// Commit of the application.
    /// </summary>
    public string? Commit { get; set; }
    
    /// <summary>
    /// User the GitHub project is under.
    /// </summary>
    public string? GitHubUser { get; set; }
    
    /// <summary>
    /// Name of the GitHub project under the user.
    /// </summary>
    public string? GitHubProject { get; set; }

    /// <summary>
    /// Tries to read the version data for the application.
    /// Version information is only added as part of the publish process. It might
    /// not be included in development builds.
    /// </summary>
    /// <returns>The version data for the application, if it is bundled.</returns>
    public static async Task<ProjectVersionData?> GetVersionDataAsync()
    {
        // Try to find the version JSON file.
        var assembly = Assembly.GetExecutingAssembly();
        await using var stream = assembly.GetManifestResourceStream("Enigma.Core.Version.json");
        if (stream == null)
        {
            return null;
        }
        
        // Read the JSON file contents.
        var versionFileContents = await new StreamReader(stream).ReadToEndAsync();
        return JsonSerializer.Deserialize<ProjectVersionData>(versionFileContents, ProjectVersionDataJsonContext.Default.ProjectVersionData);
    }
}

[JsonSerializable(typeof(ProjectVersionData))]
[JsonSourceGenerationOptions(WriteIndented=true, IncludeFields = true)]
internal partial class ProjectVersionDataJsonContext : JsonSerializerContext
{
}