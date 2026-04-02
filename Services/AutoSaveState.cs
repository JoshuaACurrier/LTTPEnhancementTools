using System.Text.Json.Serialization;

namespace LTTPEnhancementTools.Services;

public class AutoSaveState
{
    [JsonPropertyName("lastSpritePath")]
    public string LastSpritePath { get; set; } = string.Empty;

    [JsonPropertyName("lastSpritePreviewUrl")]
    public string LastSpritePreviewUrl { get; set; } = string.Empty;

    [JsonPropertyName("lastPlaylistPath")]
    public string LastPlaylistPath { get; set; } = string.Empty;

    [JsonPropertyName("lastPatchPath")]
    public string LastPatchPath { get; set; } = string.Empty;
}
