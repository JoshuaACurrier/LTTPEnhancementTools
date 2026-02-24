using System.IO;
using System.Text;

namespace LTTPEnhancementTools.Services;

public static class PcmValidator
{
    private const string ExpectedSignature = "MSU1";
    private const int HeaderSize = 8;

    /// <summary>
    /// Validates that the given file is a valid MSU-1 PCM file.
    /// Returns null if valid, or an error string if invalid.
    /// </summary>
    public static string? Validate(string filePath)
    {
        try
        {
            var info = new FileInfo(filePath);
            if (!info.Exists)
                return "File does not exist.";

            if (info.Length < HeaderSize)
                return $"File too small ({info.Length} bytes). MSU-1 header requires at least {HeaderSize} bytes.";

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var header = new byte[HeaderSize];
            int bytesRead = fs.Read(header, 0, HeaderSize);

            if (bytesRead < HeaderSize)
                return "Could not read MSU-1 header.";

            string sig = Encoding.ASCII.GetString(header, 0, 4);
            if (sig != ExpectedSignature)
                return $"Invalid MSU-1 signature. Expected \"{ExpectedSignature}\", found \"{sig}\". This may not be an MSU-1 PCM file.";

            if (info.Length <= HeaderSize)
                return "File contains only the MSU-1 header with no audio data.";

            return null; // valid
        }
        catch (Exception ex)
        {
            return $"Cannot read file: {ex.Message}";
        }
    }
}
