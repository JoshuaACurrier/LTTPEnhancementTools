using System.IO;
using System.Text;
using NAudio.Wave;

namespace LTTPEnhancementTools.Services;

/// <summary>
/// Converts common audio formats (MP3, WAV, WMA, WMV, AAC, M4A, AIFF) to MSU-1 PCM.
/// Output is always: 8-byte header ("MSU1" + uint32 LE loop point) + raw 44.1 kHz 16-bit stereo PCM.
/// </summary>
public static class PcmConverter
{
    private static readonly HashSet<string> AiffExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".aiff", ".aif" };

    /// <summary>
    /// Converts the source audio file to MSU-1 PCM format at <paramref name="destPcmPath"/>.
    /// Returns null on success, or an error message string on failure.
    /// Any partial output file is deleted on failure.
    /// </summary>
    public static async Task<string?> ConvertAsync(
        string sourcePath,
        string destPcmPath,
        uint loopPoint = 0,
        IProgress<double>? progress = null,
        CancellationToken ct = default)
    {
        return await Task.Run(() =>
        {
            try
            {
                // Choose the appropriate reader
                WaveStream reader = AiffExtensions.Contains(Path.GetExtension(sourcePath))
                    ? new AiffFileReader(sourcePath)
                    : (WaveStream)new MediaFoundationReader(sourcePath);

                using (reader)
                {
                    var targetFormat = new WaveFormat(44100, 16, 2);
                    using var resampler = new MediaFoundationResampler(reader, targetFormat)
                    {
                        ResamplerQuality = 60 // highest quality
                    };

                    // Estimate total output bytes for progress (samples × channels × bytes-per-sample)
                    long expectedBytes = reader.TotalTime.TotalSeconds > 0
                        ? (long)(reader.TotalTime.TotalSeconds * 44100 * 2 * 2)
                        : 0;

                    using var output = new FileStream(destPcmPath, FileMode.Create, FileAccess.Write, FileShare.None);

                    // Write 8-byte MSU-1 header
                    byte[] header = new byte[8];
                    Encoding.ASCII.GetBytes("MSU1").CopyTo(header, 0);
                    BitConverter.GetBytes(loopPoint).CopyTo(header, 4); // little-endian uint32
                    output.Write(header, 0, 8);

                    // Stream PCM data in 64 KB chunks
                    byte[] buffer = new byte[65536];
                    long bytesWritten = 0;
                    int lastReportedMilestone = 0;
                    int bytesRead;

                    while ((bytesRead = resampler.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ct.ThrowIfCancellationRequested();
                        output.Write(buffer, 0, bytesRead);
                        bytesWritten += bytesRead;

                        if (expectedBytes > 0 && progress != null)
                        {
                            double pct = Math.Min(1.0, (double)bytesWritten / expectedBytes);
                            // Report at 25 / 50 / 75 % milestones to avoid log spam
                            int milestone = (int)(pct * 4) * 25; // 0, 25, 50, 75, 100
                            if (milestone > lastReportedMilestone && milestone < 100)
                            {
                                progress.Report(pct);
                                lastReportedMilestone = milestone;
                            }
                        }
                    }

                    progress?.Report(1.0);
                }

                return null; // success
            }
            catch (OperationCanceledException)
            {
                TryDelete(destPcmPath);
                throw;
            }
            catch (Exception ex)
            {
                TryDelete(destPcmPath);
                return $"Conversion failed: {ex.Message}";
            }
        }, ct);
    }

    private static void TryDelete(string path)
    {
        try { File.Delete(path); } catch { /* best-effort cleanup */ }
    }
}
