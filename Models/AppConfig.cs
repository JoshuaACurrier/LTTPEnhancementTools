using System.Text.Json.Serialization;

namespace LTTPMusicReplacer.Models;

public class AppConfig
{
    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;

    [JsonPropertyName("romPath")]
    public string RomPath { get; set; } = string.Empty;

    [JsonPropertyName("tracks")]
    public Dictionary<string, string> Tracks { get; set; } = new();

    [JsonPropertyName("spritePath")]
    public string SpritePath { get; set; } = string.Empty;

    [JsonPropertyName("lastModified")]
    public string LastModified { get; set; } = string.Empty;
}
