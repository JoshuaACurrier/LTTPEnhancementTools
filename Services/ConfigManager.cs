using System.IO;
using System.Text.Json;
using LTTPEnhancementTools.Models;

namespace LTTPEnhancementTools.Services;

public static class ConfigManager
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static (AppConfig? config, string? error) Load(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<AppConfig>(json, JsonOptions);
            if (config is null)
                return (null, "Config file is empty or malformed.");
            if (config.Version != 1)
                return (null, $"Unsupported config version: {config.Version}");
            return (config, null);
        }
        catch (Exception ex)
        {
            return (null, $"Failed to load config: {ex.Message}");
        }
    }

    public static string? Save(string filePath, AppConfig config)
    {
        try
        {
            // Normalize paths to forward slashes for cross-editor readability
            config.LastModified = DateTime.UtcNow.ToString("o");
            config.RomPath = config.RomPath.Replace('\\', '/');
            config.SpritePath = config.SpritePath.Replace('\\', '/');
            var normalizedTracks = config.Tracks
                .ToDictionary(kv => kv.Key, kv => kv.Value.Replace('\\', '/'));
            config.Tracks = normalizedTracks;

            string json = JsonSerializer.Serialize(config, JsonOptions);
            File.WriteAllText(filePath, json);
            return null;
        }
        catch (Exception ex)
        {
            return $"Failed to save config: {ex.Message}";
        }
    }
}
