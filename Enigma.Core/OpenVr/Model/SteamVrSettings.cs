using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Enigma.Core.OpenVr.Model;

public class SteamVrSettings
{
    /// <summary>
    /// Dictionary of the tracker ids to their assigned role.
    /// </summary>
    [JsonPropertyName("trackers")]
    public Dictionary<string, string>? Trackers { get; set; }
}

[JsonSerializable(typeof(SteamVrSettings))]
[JsonSourceGenerationOptions(WriteIndented=true, IncludeFields = true)]
internal partial class SteamVrSettingsJsonContext : JsonSerializerContext
{
}