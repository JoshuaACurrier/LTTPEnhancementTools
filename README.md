# ALttP Enhancement Tools

A Windows desktop utility for enhancing **A Link to the Past Randomizer** runs. Manage MSU-1 music packs, replace the Link sprite, and assemble a complete customized pack in one click.

---

## Features

### Music
- **61-slot track list** — all ALttP music slots listed by name, including extended dungeon and boss tracks
- **Music Library** — a dedicated `MusicLibrary/` folder next to the app stores all your audio files in one place; songs appear in a per-slot picker popup with instant preview
- **Per-slot library picker** — click **Library** on any track row to open a popup of all your songs; hit ▶ to audition before assigning; single-click assigns to the slot
- **Audio conversion built-in** — import MP3, WAV, WMA, AAC, M4A, AIFF, and more; files are converted to MSU-1 PCM automatically and cached so the same file is never converted twice
- **Copy-to-Library prompt** — when browsing for a file manually, the app offers to copy it to your Music Library for future reuse
- **Preview playback** — listen to any assigned track before applying

### Sprite
- **Link Sprite replacement** — apply a custom `.zspr` or `.spr` Link sprite to your ROM
- **Online Sprite Browser** — browse and download sprites directly from the ALttP community sprite library without leaving the app
- **Sprite preview** — the selected sprite thumbnail is shown at the top of the window at all times

### General
- **Output file renaming** — customize the base name used for all output files (e.g. `mypack.sfc`, `mypack.msu`, `mypack-2.pcm`)
- **One-click pack assembly** — copies the ROM, generates the `.msu` marker file, and writes all numbered `.pcm` files
- **Conflict detection** — warns before overwriting existing files with Overwrite / Skip / Cancel options
- **Save / Load config** — save your track assignments and sprite as a JSON file and reload them later; sprite preview URL is preserved
- **Post-apply save prompt** — after a successful Apply, the app offers to save your settings if you haven't already
- **No admin rights required** — per-user install, no elevated permissions needed
- **No .NET runtime required** — ships as a single self-contained EXE

---

## System Requirements

- Windows 10 version 1809 (October 2018 Update) or later / Windows 11
- 64-bit (x64) processor
- ~80 MB disk space for the installed app

---

## Installation

Download the latest installer (`LTTPEnhancementToolsSetup-*-win64.exe`) from the [Releases](../../releases/latest) page and run it. No administrator password needed — it installs to your personal `AppData\Local\Programs` folder.

Alternatively, grab just the standalone `LTTPEnhancementTools.exe` and run it from anywhere.

---

## How to Use

### 1. Select your ROM
Click **Select ROM** in the toolbar and pick your ALttP Randomizer `.sfc` or `.smc` file.

### 2. (Optional) Set a Link Sprite
In the sprite header at the top:
- Click **Browse File...** to select a local `.zspr` or `.spr` sprite file
- Click **Browse Sprites...** to pick from the online ALttP sprite community library — includes a live thumbnail preview
- Click **✕** to clear the sprite (default Link will be used)

### 3. Assign music to slots

**From your Music Library (recommended):**

On first launch the app creates a `MusicLibrary/` folder next to its executable. Drop your audio files there, then:
1. Click the **Library** button on any track row
2. The popup shows all songs in your library with ▶ play buttons
3. Click ▶ to audition a song; click the song name to assign it to the slot
4. Non-converted songs are converted and cached on first assign (the cache is reused on subsequent picks)

Use **Change Location...** in the toolbar to move the Music Library folder.

**Browsing manually:**
- Click **Pick File** (or **Replace**) on any row to open a file picker
- Select any supported audio file — the app will offer to copy it to your Music Library for future reuse
- Non-PCM files are converted automatically

The ▶ button on each row previews the assigned track. Click it again to stop.

### 4. Set output folder
Click **Browse…** next to the output folder path and choose where the finished pack should be written.

### 5. Set output base name
The **Output Base Name** field controls the filename stem used for every file the app writes. It auto-fills from your ROM filename, but you can change it freely.

For example, setting it to `mypack` produces:
```
mypack.sfc
mypack.msu
mypack-2.pcm
mypack-9.pcm
…
```

### 6. Apply
Click **Apply to ROM**. The app will:
1. Copy your ROM to the output folder (using the base name you set)
2. Apply the selected Link sprite (if any)
3. Create the required empty `.msu` marker file
4. Copy/write all assigned `.pcm` files with the correct numbered names

The log panel at the bottom shows progress and any errors. After a successful apply, you'll be prompted to save your settings if you haven't already.

### 7. Save / Load
Use **Save** to write your current slot assignments and sprite to a `.json` file. Use **Load** to restore them later — handy when building multiple packs from the same base.

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

When you pick a non-PCM file from the library, the converted `.pcm` is cached in `MusicLibrary/_cache/`. Subsequent picks of the same file reuse the cache instantly. Conversion uses Windows Media Foundation — no additional software required.

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
This runs `dotnet publish` (produces a single ~70 MB self-contained EXE) and then compiles the Inno Setup installer if [Inno Setup 6](https://jrsoftware.org/isinfo.php) is installed.

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
│   ├── AppConfig.cs          # JSON config schema (tracks + sprite)
│   └── LibraryEntry.cs       # Music Library song entry model
├── Services/
│   ├── AudioPlayer.cs        # NAudio PCM playback
│   ├── PcmConverter.cs       # Audio → MSU-1 PCM conversion
│   ├── PcmValidator.cs       # MSU-1 header validation
│   ├── ConfigManager.cs      # Save/load JSON config
│   ├── MsuApplyEngine.cs     # Pack assembly engine
│   ├── SpriteApplier.cs      # ZSPR/SPR sprite patching
│   ├── MusicLibrary.cs       # Library folder scanner + cache manager
│   ├── AppSettings.cs        # App-level settings schema
│   └── SettingsManager.cs    # Persist settings to %LocalAppData%
├── Converters/
│   └── ValueConverters.cs    # WPF value converters
├── Resources/
│   ├── trackCatalog.json     # Slot number → track name mapping (61 slots)
│   ├── Styles.xaml           # Dark theme resource dictionary
│   └── icon.ico              # App icon
├── App.xaml / App.xaml.cs
├── MainWindow.xaml / MainWindow.xaml.cs
├── SpriteBrowserWindow.xaml / SpriteBrowserWindow.xaml.cs
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
