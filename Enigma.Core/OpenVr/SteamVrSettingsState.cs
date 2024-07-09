using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Enigma.Core.Diagnostic;
using Enigma.Core.OpenVr.Model;

namespace Enigma.Core.OpenVr;

public class SteamVrSettingsState
{
    /// <summary>
    /// Potential locations for SteamVR settings.
    /// </summary>
    private static readonly List<string> SteamVrSettingsPaths = new List<string>()
    {
        "C:\\Program Files (x86)\\Steam\\config\\steamvr.vrsettings",
    };
    
    /// <summary>
    /// Path of the SteamVR settings to read.
    /// </summary>
    private readonly string _filePath;

    /// <summary>
    /// Last loaded tracker roles.
    /// </summary>
    private Dictionary<string, TrackerRole>? _trackerRoles;

    /// <summary>
    /// Creates a SteamVR setting state.
    /// </summary>
    /// <param name="filePath">Path of the SteamVR settings file.</param>
    public SteamVrSettingsState(string filePath)
    {
        this._filePath = filePath;
    }

    /// <summary>
    /// Returns the SteamVR state that is relevant.
    /// </summary>
    /// <returns>SteamVR instance to read settings.</returns>
    public static SteamVrSettingsState GetState()
    {
        return new SteamVrSettingsState(SteamVrSettingsPaths.FirstOrDefault(File.Exists) ?? SteamVrSettingsPaths[0]);
    }
    
    /// <summary>
    /// Finds the TrackerRole for a SteamVR tracker string.
    /// </summary>
    /// <param name="role">SteamVR tracker role string.</param>
    /// <returns>The TrackerRole for a string, if it exists.</returns>
    public static TrackerRole? GetTrackerRole(string role)
    {
        var roleParsed = Enum.TryParse(role.Substring(12), true, out TrackerRole trackerRole);
        return roleParsed ? trackerRole : null;
    }

    /// <summary>
    /// Sets up reloading of the SteamVR settings.
    /// </summary>
    public void ConnectReloading()
    {
        // Listen to SteamVR file changes.
        var steamVrSettingsDirectory = Path.GetDirectoryName(this._filePath);
        if (steamVrSettingsDirectory != null)
        {
            var fileSystemWatcher = new FileSystemWatcher(steamVrSettingsDirectory);
            fileSystemWatcher.Changed += async (_, eventArgs) =>
            {
                if (eventArgs.Name != "steamvr.vrsettings") return;
                await this.ReloadSettingsAsync();
            };
            fileSystemWatcher.EnableRaisingEvents = true;
        }
        
        // Occasionally reload the file.
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(5000);
                await this.ReloadSettingsAsync();
            }
        });
        
        // Load the initial settings.
        this.ReloadSettingsAsync().Wait();
    }

    /// <summary>
    /// Reload the settings.
    /// </summary>
    /// <param name="steamVrSettings">Updated SteamVR settings.</param>
    public void ReloadSettings(SteamVrSettings? steamVrSettings)
    {
        var oldTrackers = this._trackerRoles ?? new Dictionary<string, TrackerRole>();
        var newTrackers = new Dictionary<string, TrackerRole>();
        if (steamVrSettings?.Trackers != null)
        {
            foreach (var (trackerId, trackerRoleString) in steamVrSettings.Trackers)
            {
                var trackerRole = GetTrackerRole(trackerRoleString);
                if (!trackerRole.HasValue)
                {
                    Logger.Error($"Unsupported tracker role bound for {trackerId}: \"{trackerRoleString}\"");
                    continue;
                }
                newTrackers[trackerId] = trackerRole.Value;
                if (!oldTrackers.TryGetValue(trackerId, out var existingTrackRole) || existingTrackRole != trackerRole.Value)
                {
                    Logger.Debug($"Detected tracker {trackerId} as {trackerRole.Value}.");
                }
            }
        }
        this._trackerRoles = newTrackers;
    }
    
    /// <summary>
    /// Attempts to reload the settings.
    /// </summary>
    public async Task ReloadSettingsAsync()
    {
        try
        {
            var steamVrSettings = JsonSerializer.Deserialize<SteamVrSettings>(await File.ReadAllTextAsync(this._filePath), SteamVrSettingsJsonContext.Default.SteamVrSettings);
            this.ReloadSettings(steamVrSettings);
        }
        catch (Exception)
        {
            // File may not have been able to be read if mid-write.
        }
    }

    /// <summary>
    /// Returns the role for a tracker.
    /// </summary>
    /// <param name="hardwareId">Hardware id of the tracker to find.</param>
    /// <returns>Role of the tracker, if it is defined.</returns>
    public TrackerRole GetRole(string hardwareId)
    {
        if (this._trackerRoles == null) return TrackerRole.None;
        return this._trackerRoles.GetValueOrDefault(hardwareId, TrackerRole.None);
    }

    /// <summary>
    /// Returns all the current tracker roles.
    /// </summary>
    /// <returns>All of the stored tracker roles.</returns>
    public Dictionary<string, TrackerRole> GetAllTrackerRoles()
    {
        var trackerRoles = new Dictionary<string, TrackerRole>();
        if (this._trackerRoles == null) return trackerRoles;
        foreach (var (trackerName, trackerRole) in this._trackerRoles)
        {
            trackerRoles[trackerName] = trackerRole;
        }
        return trackerRoles;
    }
}