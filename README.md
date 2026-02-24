# ALttP MSU-1 Music Switcher

A Windows desktop utility for managing MSU-1 music packs for the **A Link to the Past Randomizer**. Pick audio files for each of the 61 in-game music slots, preview them, and assemble a complete MSU-1 pack in one click.

---

## Features

- **61-slot track list** — all ALttP music slots listed by name, including extended dungeon and boss tracks
- **Output file renaming** — customize the base name used for all output files before applying (e.g. `mypack.sfc`, `mypack.msu`, `mypack-2.pcm`)
- **Audio conversion built-in** — import MP3, WAV, WMA, AAC, M4A, AIFF, and more; the app converts them to MSU-1 PCM format automatically
- **Preview playback** — listen to any assigned track before applying
- **One-click pack assembly** — copies the ROM, generates the `.msu` marker file, and writes all numbered `.pcm` files
- **Conflict detection** — warns before overwriting existing files with Overwrite / Skip / Cancel options
- **Config save/load** — save your track assignments as a JSON file and reload them later
- **No admin rights required** — per-user install, no elevated permissions needed
- **No .NET runtime required** — ships as a single self-contained EXE

---

## System Requirements

- Windows 10 version 1809 (October 2018 Update) or later / Windows 11
- 64-bit (x64) processor
- ~200 MB disk space for the installed app

---

## Installation

Download the latest installer (`LTTPEnhancementToolsSetup-*-win64.exe`) from the [Releases](../../releases/latest) page and run it. No administrator password needed — it installs to your personal `AppData\Local\Programs` folder.

Alternatively, grab just the standalone `LTTPEnhancementTools.exe` and run it from anywhere.

---

## How to Use

### 1. Select your ROM
Click **Select ROM** in the toolbar and pick your ALttP Randomizer `.sfc` or `.smc` file.

### 2. Assign audio to slots
For each track slot you want to replace:
- Click **Pick File** (or **Replace**) to open a file picker
- Select any supported audio file (see [Supported Formats](#supported-audio-formats) below)
- If you pick a non-PCM file, it is automatically converted to MSU-1 PCM and saved next to the source file

The ▶ button previews the assigned track. Click it again (or pick a different slot) to stop.

### 3. Set output folder
Click **Browse…** next to the output folder path and choose where the finished pack should be written.

### 4. Set output base name
The **Output Base Name** field controls the filename stem used for every file the app writes. It auto-fills from your ROM filename, but you can change it freely.

For example, setting it to `mypack` produces:
```
mypack.sfc
mypack.msu
mypack-2.pcm
mypack-9.pcm
…
```

### 5. Apply
Click **Apply to ROM**. The app will:
1. Copy your ROM to the output folder (using the base name you set)
2. Create the required empty `.msu` marker file
3. Copy/write all assigned `.pcm` files with the correct numbered names

The log panel at the bottom shows progress and any errors.

### 6. Save / Load config
Use **Save Config** to write your current slot assignments to a `.json` file. Use **Load Config** to restore them later — handy when building multiple packs from the same base.

---

## Supported Audio Formats

| Format | Extensions |
|--------|-----------|
| MSU-1 PCM (direct, no conversion) | `.pcm` |
| MP3 | `.mp3` |
| WAV | `.wav` |
| Windows Media Audio / Video | `.wma`, `.wmv` |
| AAC / MPEG-4 Audio | `.aac`, `.m4a`, `.mp4` |
| AIFF | `.aiff`, `.aif` |

Converted files are saved as `.pcm` next to the original (e.g. `overworld.mp3` → `overworld.pcm`). Conversion uses Windows Media Foundation — no additional software required.

> **Note:** FLAC and OGG/Vorbis are not currently supported. Convert them to MP3 or WAV first using a free tool like [Audacity](https://www.audacityteam.org/) or [fre:ac](https://www.freac.org/).

---

## MSU-1 PCM Format

MSU-1 PCM files use the following layout:

| Bytes | Content |
|-------|---------|
| 0–3 | ASCII signature: `MSU1` |
| 4–7 | Loop point (uint32, little-endian) — `0` = loop from beginning |
| 8+ | Raw PCM: 44,100 Hz · 16-bit signed · stereo interleaved |

---

## Troubleshooting

If the app crashes or fails to start, a crash log is automatically written to:

```
%LocalAppData%\LTTPEnhancementTools\crash.log
```

Please include the contents of this file when reporting an issue.

---

## Building from Source

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10/11 (WPF requires Windows)

### Run in development
```bash
dotnet run
```

### Build self-contained installer EXE
```bash
publish.bat
```
This runs `dotnet publish` (produces a single ~155 MB self-contained EXE) and then compiles the Inno Setup installer if [Inno Setup 6](https://jrsoftware.org/isinfo.php) is installed.

Or publish manually:
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
# Output: bin\Release\net8.0-windows\win-x64\publish\LTTPEnhancementTools.exe
```

### Project structure
```
LTTPEnhancementTools/
├── Models/
│   ├── TrackSlot.cs          # Per-slot data + validation state
│   └── AppConfig.cs          # JSON config schema
├── Services/
│   ├── AudioPlayer.cs        # NAudio PCM playback
│   ├── PcmConverter.cs       # Audio → MSU-1 PCM conversion
│   ├── PcmValidator.cs       # MSU-1 header validation
│   ├── ConfigManager.cs      # Save/load JSON config
│   └── MsuApplyEngine.cs     # Pack assembly engine
├── Converters/
│   └── ValueConverters.cs    # WPF value converters
├── Resources/
│   ├── trackCatalog.json     # Slot number → track name mapping (61 slots)
│   ├── Styles.xaml           # Dark theme resource dictionary
│   └── icon.ico              # App icon
├── App.xaml / App.xaml.cs
├── MainWindow.xaml / MainWindow.xaml.cs
├── LTTPEnhancementTools.csproj
├── setup.iss                 # Inno Setup installer script
└── publish.bat               # One-command build + package script
```

### Dependencies
- [NAudio 2.2.1](https://github.com/naudio/NAudio) — audio decoding, resampling, and playback
- Windows Media Foundation (built into Windows) — MP3/AAC/WMA/WMV decoding

---

## License

This project is provided as-is for personal use. ALttP and its assets are property of Nintendo. This tool does not include or distribute any copyrighted game assets.
