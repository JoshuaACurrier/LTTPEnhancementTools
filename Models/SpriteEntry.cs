using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LTTPEnhancementTools.Models;

public class SpriteEntry
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    [JsonPropertyName("file")]
    public string File { get; set; } = string.Empty;

    [JsonPropertyName("preview")]
    public string Preview { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    [JsonPropertyName("usage")]
    public List<string> Usage { get; set; } = new();
}
