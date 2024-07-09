using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Enigma.Core.Diagnostic;

namespace Enigma.Core.Roblox;

public static class RobloxPlugins
{
    /// <summary>
    /// Name of the plugin file to copy from.
    /// </summary>
    public const string SourcePluginName = "EnigmaCompanionPlugin.rbxmx";
    
    /// <summary>
    /// Name of the plugin file to copy to.
    /// </summary>
    public const string TargetSourcePluginName = "ManagedEnigmaCompanionPlugin.rbxmx";
    
    /// <summary>
    /// List of plugin folders.
    /// </summary>
    private static readonly List<string> PluginFolders = new List<string>()
    {
        Path.Combine(Environment.GetEnvironmentVariable("LocalAppData") ?? "", "Roblox", "Plugins"),
    };

    /// <summary>
    /// Copies the managed plugin for Roblox Studio.
    /// </summary>
    public static async Task CopyPluginAsync()
    {
        // Return if the source plugin does not exist.
        var sourcePluginPath = Path.Combine(AppContext.BaseDirectory, SourcePluginName);
        if (!File.Exists(sourcePluginPath))
        {
            Logger.Debug($"Source plugin file {sourcePluginPath} not found. The managed plugin will not be copied.");
            return;
        }
        
        // Copy the plugins.
        var sourceFileHash = Convert.ToHexString(SHA256.HashData(await File.ReadAllBytesAsync(sourcePluginPath)));
        foreach (var pluginFolder in PluginFolders)
        {
            if (!Directory.Exists(pluginFolder)) continue;
            var targetPluginPath = Path.Combine(pluginFolder, TargetSourcePluginName);
            if (File.Exists(targetPluginPath))
            {
                // Ignore the path if the file is already up-to-date.
                var targetFileHash = Convert.ToHexString(SHA256.HashData(await File.ReadAllBytesAsync(targetPluginPath)));
                if (sourceFileHash == targetFileHash)
                {
                    Logger.Debug("Managed plugin is already up-to-date.");
                    continue;
                }
                
                // Clear the existing plugin and copy the new plugin.
                Logger.Info("Updating managed companion plugin for Roblox Studio.");
                if (File.Exists(targetPluginPath))
                {
                    File.Delete(targetPluginPath);
                }
                File.Copy(sourcePluginPath, targetPluginPath);
            }
            else
            {
                // Copy the new plugin file.
                Logger.Info("Copying managed companion plugin for Roblox Studio.");
                File.Copy(sourcePluginPath, targetPluginPath);
            }
        }
    }
}