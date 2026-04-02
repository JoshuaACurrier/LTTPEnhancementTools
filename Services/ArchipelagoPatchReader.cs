using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using LTTPEnhancementTools.Models;

namespace LTTPEnhancementTools.Services;

public static class ArchipelagoPatchReader
{
    public static (ArchipelagoMetadata? metadata, string? error) ReadPatch(string aplttpPath)
    {
        if (!File.Exists(aplttpPath))
            return (null, $"Patch file not found: {aplttpPath}");

        try
        {
            using var zip = ZipFile.OpenRead(aplttpPath);
            var entry = zip.GetEntry("archipelago.json");
            if (entry is null)
                return (null, "Invalid .aplttp file: missing archipelago.json");

            using var stream = entry.Open();
            var json = JsonSerializer.Deserialize<ArchipelagoJson>(stream, JsonDefaults.ReadOnly);
            if (json is null)
                return (null, "Failed to parse archipelago.json");

            string dir = Path.GetDirectoryName(aplttpPath)!;
            string stem = Path.GetFileNameWithoutExtension(aplttpPath);
            string sfcPath = Path.Combine(dir, stem + ".sfc");

            var metadata = new ArchipelagoMetadata(
                Server: json.Server ?? string.Empty,
                Player: json.Player,
                PlayerName: json.PlayerName ?? string.Empty,
                Game: json.Game ?? string.Empty,
                PatchFilePath: aplttpPath,
                ExpectedSfcPath: sfcPath
            );

            return (metadata, null);
        }
        catch (InvalidDataException)
        {
            return (null, "File is not a valid .aplttp archive.");
        }
        catch (Exception ex)
        {
            return (null, $"Error reading patch: {ex.Message}");
        }
    }

    private class ArchipelagoJson
    {
        [JsonPropertyName("server")]
        public string? Server { get; set; }

        [JsonPropertyName("player")]
        public int Player { get; set; }

        [JsonPropertyName("player_name")]
        public string? PlayerName { get; set; }

        [JsonPropertyName("game")]
        public string? Game { get; set; }
    }
}
