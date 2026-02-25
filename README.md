# ALttP Enhancement Tools

![LTTP Enhanced](Resources/lttpEnhancedLogo.png)

A Windows desktop utility for enhancing **A Link to the Past Randomizer** runs. Manage MSU-1 music packs, replace the Link sprite, and launch everything — emulator, SNI, and community tracker — in one click.

> **Bring Your Own ROM.** This tool requires you to provide your own legally-obtained ALttP ROM. It does not include, distribute, or circumvent any copyrighted game data. See [Legal & Copyright](#legal--copyright) below.

---

## Features

### Music
- **61-slot track list** — all ALttP music slots listed by name, including extended dungeon and boss tracks
- **Music Library** — a dedicated `MusicLibrary/` folder next to the app stores all your audio files in one place; songs appear in a per-slot picker popup with instant preview
- **Per-slot library picker** — click **Library** on any track row to open a popup of all your songs; hit ▶ to audition before assigning; single-click assigns to the slot
- **Audio conversion built-in** — import MP3, WAV, WMA, AAC, M4A, AIFF, and more; files are converted to MSU-1 PCM automatically and cached so the same file is never converted twice
- **Copy-to-Library prompt** — when browsing for a file manually, the app offers to copy it to your Music Library for future reuse
- **Preview playback** — listen to any assigned track before applying
- **Track search** — filter the 61-slot track list by name to find slots quickly; **Clear All** removes all assignments at once
- **Playlists** — save and load your full set of track assignments as `.json` playlist files; auto-saved between sessions
- **Pack Export / Import** — bundle your assigned PCM tracks into a shareable `.lttppack` file and share it with others; import a pack to instantly load all its tracks

### Sprite
- **Link Sprite replacement** — apply a custom `.zspr` or `.spr` Link sprite to your ROM
- **Online Sprite Browser** — browse 600+ community sprites in a card grid with live preview thumbnails; animated triforce loading indicator while images load
- **Favorites** — star any sprite to pin it to the top of the list; favorites persist across sessions
- **Offline support** — sprite list and all preview images are cached to disk after first load; the browser works offline using the local cache; **↻ Refresh** button re-fetches from the server when you want updates
- **Default Link preview** — Link's sprite is shown automatically when no custom sprite is selected; downloaded once and cached for subsequent launches
- **Reset to default** — selecting "Link" in the browser clears any custom sprite and restores the default
- **Sprite preview** — the selected sprite thumbnail is shown at the top of the window at all times

### Archipelago / One-Click Launch
- **Setup Wizard** — on first run a 4-step wizard walks you through configuring your emulator, Archipelago connector script, SNI, and community tracker
- **One-click launch** — after applying, a single **Launch ROM** button starts SNI, opens your community tracker in the browser, and launches the emulator with the ROM and Lua connector pre-loaded
- **BizHawk integration** — auto-passes `--lua=<connector>` before the ROM path so the Archipelago Lua script starts automatically
- **SNI auto-start** — SNI.exe is started before the emulator if it isn't already running
- **Community tracker** — choose Dunka's Tracker or alttprtracker.com from a dropdown; opens in the browser on every launch
- **Re-run wizard** — reconfigure any path at any time via the **⚙ Run Setup Wizard…** button in Launch Settings

### General
- **Output file renaming** — customize the base name used for all output files (e.g. `mypack.sfc`, `mypack.msu`, `mypack-2.pcm`)
- **Dedicated output folder** — a persistent `Output/` folder next to the app holds all generated packs; change it any time via Browse; 🗑 button clears the folder when you're done
- **One-click pack assembly** — copies the ROM, generates the `.msu` marker file, and writes all numbered `.pcm` files
- **Sprite-only apply** — apply just a Link sprite to a ROM with no music changes required
- **Auto-save session** — your last ROM, sprite, and playlist are remembered and restored on next launch
- **No admin rights required** — per-user install, no elevated permissions needed
- **No .NET runtime required** — ships as a single self-contained EXE

---

## System Requirements

- Windows 10 version 1809 (October 2018 Update) or later / Windows 11
- 64-bit (x64) processor
- ~80 MB disk space for the installed app

> ⚠️ **You must provide your own legally-obtained ALttP ROM** (`.sfc` or `.smc`). This tool does not include any ROM files or copyrighted game assets.

---

## Installation

Download the latest installer (`LTTPEnhancementToolsSetup-*-win64.exe`) from the [Releases](../../releases/latest) page and run it. No administrator password needed — it installs to your personal `AppData\Local\Programs` folder.

Alternatively, grab just the standalone `LTTPEnhancementTools.exe` and run it from anywhere.

> **Windows SmartScreen:** On first launch Windows may show a "Windows protected your PC" warning because the app is not yet code-signed. Click **More info → Run anyway**. This is normal for independent open-source tools. See the [FAQ](HELP.md#faq) for more details.

---

## How to Use

### 1. First-run Setup Wizard
On first launch a wizard appears to configure your launch settings:
- **Step 1** — Introduction
- **Step 2** — Select your emulator EXE (e.g. `EmuHawk.exe` for BizHawk)
- **Step 3** — Select the Archipelago Lua connector script (optional; typically `connector_bizhawk_emuHawk.lua` inside your Archipelago install)
- **Step 4** — Select `SNI.exe` and choose a community tracker

You can skip the wizard and configure paths later from the **Launch Settings** panel, or re-run it at any time with the **⚙ Run Setup Wizard…** button.

### 2. Select your ROM
Click **Select ROM** and pick your ALttP Randomizer `.sfc` or `.smc` file.

### 3. (Optional) Set a Link Sprite
In the sprite header at the top:
- Click **Browse File...** to select a local `.zspr` or `.spr` sprite file
- Click **Browse Sprites...** to open the online sprite browser — search, star favorites, and click **Select Sprite** to download and apply; selecting "Link" resets to the default sprite
- Click **✕** to clear the sprite (default Link will be used)

The sprite browser caches the full sprite list and all preview images to disk after the first load, so it opens instantly and works offline on subsequent uses. Use the **↻ Refresh** button to check for new sprites.

### 4. Assign music to slots

**From your Music Library (recommended):**

On first launch the app creates a `MusicLibrary/` folder next to its executable. Drop your audio files there, then:
1. Click the **Library** button on any track row
2. The popup shows all songs in your library with ▶ play buttons
3. Click ▶ to audition a song; click the song name to assign it to the slot

Use **Change Location...** in the toolbar to move the Music Library folder.

**Browsing manually:**
- Click **Pick File** (or **Replace**) on any row to open a file picker
- Non-PCM files are converted to MSU-1 PCM automatically

### 5. Save / Load Playlists

Your track assignments can be saved as a **playlist** (`.json`) for later reuse:
- Click **Save Playlist** to save your current assignments to a `.json` file (stored in `MusicLibrary/Playlists/`)
- Click **Load Playlist** to restore a saved set of assignments

**Sharing music packs with others:**
- Click **Export Pack…** to bundle all your assigned PCM files into a single `.lttppack` archive
- Share the `.lttppack` file with anyone — they can click **Import Pack…** to extract the PCMs and load the playlist in one step
- Missing files are skipped on export with a warning; re-importing the same pack is safe (existing files are not overwritten)

### 6. Set output folder and name
The app automatically creates an `Output/` folder next to its executable. Click **Browse…** to choose a different folder. The 🗑 button clears it when you're done.

The **Output Base Name** field controls the filename stem (auto-fills from your ROM filename).

### 7. Apply
Click **Apply to ROM**. The app will:
1. Copy your ROM to the output folder
2. Apply the selected Link sprite (if any)
3. Create the required empty `.msu` marker file
4. Write all assigned `.pcm` files with the correct numbered names

You can apply with just a sprite, just music, or both.

### 8. Launch
After a successful apply, click **Launch ROM**. The app will:
1. Start **SNI.exe** (if configured and not already running)
2. Open your selected **community tracker** in the browser
3. Launch the **emulator** with the ROM and Lua connector script

If no emulator is configured, the ROM opens in whatever app is associated with `.sfc` files on your system.

---

## Pack Export / Import (`.lttppack`)

The `.lttppack` format lets you share a complete music setup with other players.

| What's included | What's NOT included |
|-----------------|---------------------|
| All assigned PCM audio files | Your ROM file |
| Track-to-slot mapping | Any sprite |
| Playlist name | |

**Exporting:** Click **Export Pack…** in the MUSIC header. Choose a destination. If any assigned file is missing from disk, it is skipped (you'll see a warning showing how many were omitted — the archive is still valid for the tracks that were present).

**Importing:** Click **Import Pack…**. PCM files are extracted to `MusicLibrary/Imported/<pack name>/`. Your current assignments are replaced by the pack's assignments. The session is left as "unsaved" so you can rename/save it as your own playlist.

**Re-importing** the same pack is safe — files that already exist in the destination are not overwritten.

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

Settings are stored in two separate files:
```
%LocalAppData%\LTTPEnhancementTools\launchSettings.json   ← emulator, SNI, tracker paths
%LocalAppData%\LTTPEnhancementTools\settings.json         ← library/output folder paths
```

Delete `launchSettings.json` to force the setup wizard to reappear on next launch.

Please include the crash log when reporting an issue. For common questions, see [HELP.md](HELP.md).

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
│   ├── TrackSlot.cs              # Per-slot data + validation state
│   ├── Playlist.cs               # Playlist model (name + slot→path map)
│   ├── LibraryEntry.cs           # Music Library song entry model
│   └── SpriteEntry.cs            # Sprite browser entry model
├── Services/
│   ├── AudioPlayer.cs            # NAudio PCM playback
│   ├── PcmConverter.cs           # Audio → MSU-1 PCM conversion
│   ├── PcmValidator.cs           # MSU-1 header validation
│   ├── MsuApplyEngine.cs         # Pack assembly engine
│   ├── SpriteApplier.cs          # ZSPR/SPR sprite patching
│   ├── MusicLibrary.cs           # Library folder scanner + cache manager
│   ├── PlaylistManager.cs        # Save/load JSON playlists
│   ├── PlaylistBundleManager.cs  # Export/import .lttppack archives
│   ├── AutoSaveManager.cs        # Persist last session state
│   ├── FavoritesManager.cs       # Persist sprite favorites
│   ├── AppSettings.cs            # App-level settings (library/output paths)
│   ├── SettingsManager.cs        # Persist AppSettings to %LocalAppData%
│   ├── LaunchSettings.cs         # Launch settings (emulator, SNI, tracker)
│   └── LaunchSettingsManager.cs  # Persist LaunchSettings to %LocalAppData%
├── Converters/
│   └── ValueConverters.cs        # WPF value converters
├── Resources/
│   ├── trackCatalog.json         # Slot number → track name mapping (61 slots)
│   ├── Styles.xaml               # Dark theme resource dictionary
│   └── icon.ico                  # App icon
├── Controls/
│   └── SpriteImageControl.xaml/.cs   # Sprite card thumbnail with triforce loading animation
├── App.xaml / App.xaml.cs
├── MainWindow.xaml / MainWindow.xaml.cs
├── SetupWizardWindow.xaml / SetupWizardWindow.xaml.cs
├── SpriteBrowserWindow.xaml / SpriteBrowserWindow.xaml.cs
├── LTTPEnhancementTools.csproj
├── setup.iss                     # Inno Setup installer script
└── publish.bat                   # One-command build + package script
```

### Dependencies
- [NAudio 2.2.1](https://github.com/naudio/NAudio) — audio decoding, resampling, and playback
- Windows Media Foundation (built into Windows) — MP3/AAC/WMA/WMV decoding

---

## Legal & Copyright

**ALttP and all its assets** (music, graphics, sprites, ROM) are the exclusive property of Nintendo Co., Ltd. This project has no affiliation with Nintendo.

**This tool does not include, bundle, or distribute any copyrighted game content**, including but not limited to:
- ROM files
- Game music or audio data
- Sprite or graphics data
- Any other Nintendo-owned intellectual property

**Users are solely responsible** for ensuring they legally own any ROM they modify with this tool, and for complying with all applicable laws regarding backup copies of software they own.

The community sprite repository at [alttpr.com](https://alttpr.com) is maintained independently by the ALttP Randomizer community. Sprites displayed in the browser are the work of their respective creators.

This software is provided as-is for personal, non-commercial use. The authors make no warranties and accept no liability for any damages arising from its use.
