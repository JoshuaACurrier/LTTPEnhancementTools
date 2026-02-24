using System.IO;

namespace LTTPEnhancementTools.Models;

public class LibraryEntry
{
    private string _sourcePath = string.Empty;
    private string _ext = string.Empty; // cached lowercase extension including dot

    public string Name { get; set; } = string.Empty;  // filename without extension

    public string SourcePath
    {
        get => _sourcePath;
        set { _sourcePath = value; _ext = Path.GetExtension(value).ToLowerInvariant(); }
    }

    public string? CachedPcmPath { get; set; } // null = not yet cached or source is newer

    public bool   IsPcm           => _ext == ".pcm";
    public string FormatTag        => _ext.TrimStart('.').ToUpperInvariant();
    public string AssignablePath   => CachedPcmPath ?? _sourcePath;
    public bool   NeedsConversion  => !IsPcm && CachedPcmPath is null;
    public bool   IsCached         => CachedPcmPath is not null;
}
