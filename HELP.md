# Archipelago LTTP Enhancer — Help & FAQ

---

## Getting Started

### What do I need to use this tool?
- **Windows 10 (1809+) or Windows 11**, 64-bit
- **Your own ALttP ROM** — the app does not include one. You need a legally-obtained copy of *A Link to the Past* (`.sfc` or `.smc`). Archipelago generates a patched ROM from your `.aplttp` patch file.
- **Music files** (optional) — MP3, WAV, M4A, AAC, WMA, or PCM files you want to use as in-game music
- **A Link sprite** (optional) — a `.zspr` file from [alttpr.com/sprites](https://alttpr.com/sprites) or elsewhere

### How do I install it?
Download `LTTPEnhancementToolsSetup-*-win64.exe` from the [Releases](../../releases/latest) page and run it. No administrator password required — it installs per-user.

You can also just download `LTTPEnhancementTools.exe` and run it directly without installing.

### What happens on first launch?
A **setup wizard** appears. It walks you through four steps:
1. Introduction
2. Select your **emulator** (e.g. `EmuHawk.exe` for BizHawk)
3. Select your **Archipelago Lua connector script** (optional — only needed for Archipelago randomizer)
4. Select **SNI.exe** and choose a **community tracker**

You can skip any step and configure them later from the Launch Settings panel. Re-run the wizard any time with **⚙ Run Setup Wizard…**.

---

## Applying Music

### What audio formats are supported?
MP3, WAV, WMA, WMV, AAC, M4A, MP4, AIFF. Files are automatically converted to MSU-1 PCM format.

> FLAC and OGG are not supported — convert them to MP3 or WAV first using [Audacity](https://www.audacityteam.org/) or [fre:ac](https://www.freac.org/).

### How does conversion work?
When you assign a non-PCM file, the app converts it to MSU-1 PCM using Windows Media Foundation (built into Windows — no extra install needed). The converted file is cached in `MusicLibrary/_cache/` so the same source file is never converted twice.

### Where should I put my music files?
Drop them in the `MusicLibrary/` folder next to the app. They'll appear in the **Library** picker popup on every track row. You can also use **Change Location...** to point the library at a different folder.

### What is a loop point?
The loop point is a sample offset within the PCM file where the track loops back to when it reaches the end. If you import an existing PCM file, its loop point is preserved. If you convert from another format, the loop point is set to 0 (loops from the beginning). To set a custom loop point, use a tool like [MSU1-Creator](https://github.com/qwertymodo/msu1creator) before importing.

### What is a playlist?
A playlist (`.json`) saves your complete set of track assignments by name. Use **Save Playlist** and **Load Playlist** in the MUSIC header. Playlists are stored in `MusicLibrary/Playlists/` by default. Your last-used playlist is automatically restored on next launch.

---

## Sharing Music (`.lttppack`)

### What is a `.lttppack` file?
A `.lttppack` is a ZIP archive containing:
- All the PCM audio files assigned to slots in your playlist
- A `manifest.json` describing which slot each file maps to and the playlist name

It lets you share a complete music setup with another player — they don't need to have your files or know which file goes in which slot.

### How do I export a pack?
Assign your tracks, then click **Export Pack…** in the MUSIC header. Choose a destination for the `.lttppack` file. If any assigned track's file is missing from disk, it is skipped with a warning — the archive is still valid for the files that were present.

### How do I import a pack someone sent me?
Click **Import Pack…** and select the `.lttppack` file. The PCM files are extracted to `MusicLibrary/Imported/<pack name>/` and all 61 slots are loaded from the pack. Your existing assignments are replaced. The session is left as "unsaved" — use **Save Playlist** if you want to keep these assignments as your own playlist.

### Can I re-import the same pack?
Yes — re-importing is safe and idempotent. Files that already exist in the destination folder are not overwritten.

---

## Sprites

### What sprite format does the app use?
The app supports `.zspr` (the modern community format used by alttpr.com) and `.spr` (legacy raw format). `.zspr` includes both graphics data and palette data.

### How do I browse community sprites?
Click **Browse Sprites...** in the sprite header. The app fetches the full list from [alttpr.com/sprites](https://alttpr.com) and displays them as thumbnail cards. Search by name, star favorites with ⭐, and click **Select Sprite** to download and apply.

### Does the sprite browser work offline?
Yes — after the first load, both the sprite list and all preview images are cached to disk. Subsequent opens load instantly from the cache and work fully offline. Click **↻ Refresh** to re-fetch the list from the server when you want to pick up newly added sprites.

Cached files are stored in:
```
%LocalAppData%\LTTPEnhancementTools\SpriteCache\
```

### How do I go back to the default Link sprite?
Either click **✕** in the sprite header, or open the sprite browser, scroll to **Link** at the top of the list, and click **Select Sprite**. Either action clears the custom sprite and restores the default.

### Where are my favorites saved?
```
%LocalAppData%\LTTPEnhancementTools\sprite_favorites.json
```

---

## Troubleshooting

### The app won't start / crashes immediately
Check the crash log:
```
%LocalAppData%\LTTPEnhancementTools\crash.log
```
Include this file when reporting an issue.

### The ROM won't load / "file not found" error
- Make sure the `.sfc`/`.smc` file exists at the path shown
- If you moved the ROM after selecting it, use **Open Patch...** to re-point to the new location

### Tracks aren't playing in-game
- Confirm your emulator supports MSU-1. BizHawk and Snes9x (recent versions) both do.
- Make sure the `.msu` marker file, the ROM, and all `.pcm` files are in the **same folder** with the **same base name** (e.g. `mypack.sfc`, `mypack.msu`, `mypack-2.pcm`)
- In BizHawk: Confirm the Lua connector script is running (check the Lua console)

### The sprite isn't showing in-game
- The sprite must be in `.zspr` or `.spr` format
- Make sure you clicked **Enhance & Launch** after selecting the sprite — selecting it in the UI does not patch the ROM until you apply

### The sprite browser shows loading animations but no images
This usually means the app can't reach the internet. Check your network connection. If the browser has loaded before, the cached images should still appear.

### I reset my PC / reinstalled and lost my settings
Settings and cache are stored in:
```
%LocalAppData%\LTTPEnhancementTools\
```
Back up this folder to preserve your sprite favorites, cached images, and settings.

To reconfigure from scratch, delete `launchSettings.json` to trigger the first-run wizard on next launch.

---

## FAQ

### Windows says "Windows protected your PC" when I try to run it. Is it safe?

Yes. This warning ("Windows SmartScreen") appears for any downloaded EXE that isn't digitally code-signed by a registered publisher. The app is open source — you can review every line of code in this repository.

To run it: click **More info**, then **Run anyway**.

This warning will become less frequent as the app builds download reputation with Microsoft's SmartScreen service. For users who want an immediate fix, the developers are exploring open-source code signing via [SignPath.io](https://signpath.io).

### Does this tool work with non-randomizer ALttP ROMs?
Yes — any unheadered NTSC ALttP ROM (`.sfc`/`.smc`) should work. The sprite injection targets fixed ROM addresses from the pyz3r reference implementation.

### Can I use this for other SNES games?
No. The track catalog, sprite injection offsets, and MSU-1 slot numbering are specific to A Link to the Past.

### Where are my settings stored?
```
%LocalAppData%\LTTPEnhancementTools\launchSettings.json   ← emulator/SNI/tracker paths
%LocalAppData%\LTTPEnhancementTools\settings.json         ← library/output folder paths
%LocalAppData%\LTTPEnhancementTools\autoSave.json         ← last session (ROM, sprite, playlist)
%LocalAppData%\LTTPEnhancementTools\sprite_favorites.json ← starred sprites
```

### Can I move my Music Library to a different drive?
Yes — click **Change Location...** in the Music Library toolbar and point it to any folder. The app updates `settings.json` with the new path. Your previously-converted PCM cache stays in the old `_cache/` subfolder; re-assign files from the new location to rebuild it.

### Can I contribute a new feature or report a bug?
Open an issue or pull request on [GitHub](../../issues). Please include the crash log (`%LocalAppData%\LTTPEnhancementTools\crash.log`) for any bug reports.

---

## File & Folder Reference

| Location | Contents |
|----------|----------|
| Next to EXE: `MusicLibrary/` | Your source audio files |
| `MusicLibrary/_cache/` | Converted PCM files (auto-generated) |
| `MusicLibrary/Playlists/` | Saved playlist `.json` files |
| `MusicLibrary/Imported/<name>/` | PCM files extracted from `.lttppack` imports |
| `Output/` | Generated ROM + MSU + PCM packs |
| `%LocalAppData%\LTTPEnhancementTools\` | App settings, crash log, sprite cache |
| `%LocalAppData%\LTTPEnhancementTools\SpriteCache\` | Cached `.zspr` sprite files |
| `%LocalAppData%\LTTPEnhancementTools\SpriteCache\Previews\` | Cached preview thumbnails |
