using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LTTPEnhancementTools.Models;
using LTTPEnhancementTools.Services;
using Microsoft.Win32;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace LTTPEnhancementTools;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    // ── Observable state ──────────────────────────────────────────────────
    private string? _romPath;
    private string? _romBaseName;
    private string? _outputBaseName;
    private string? _outputDir;
    private string? _spritePath;
    private bool _isApplying;
    private bool _isConverting;

    public ObservableCollection<TrackSlot> Tracks { get; } = new();

    public string? RomBaseName
    {
        get => _romBaseName;
        private set { _romBaseName = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasRom)); OnPropertyChanged(nameof(CanApply)); }
    }

    public string? OutputBaseName
    {
        get => _outputBaseName;
        set { _outputBaseName = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanApply)); }
    }

    public string? OutputDir
    {
        get => _outputDir;
        private set { _outputDir = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanApply)); }
    }

    public string? SpritePath
    {
        get => _spritePath;
        set { _spritePath = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasSprite)); OnPropertyChanged(nameof(SpriteDisplayName)); }
    }

    public bool HasSprite => _spritePath is not null;

    public string? SpriteDisplayName => _spritePath is null
        ? null
        : Path.GetFileNameWithoutExtension(_spritePath);

    public bool HasRom => _romPath is not null;

    public bool CanApply => _romPath is not null
                         && _outputDir is not null
                         && Tracks.Any(t => t.HasFile)
                         && !string.IsNullOrWhiteSpace(_outputBaseName)
                         && !_isApplying
                         && !_isConverting;

    public string AssignedCountText
    {
        get
        {
            int count = Tracks.Count(t => t.HasFile);
            return count == 0 ? string.Empty : $"{count} track{(count != 1 ? "s" : "")} assigned";
        }
    }

    // ── Services ─────────────────────────────────────────────────────────
    private readonly AudioPlayer _audio = new();
    private TrackSlot? _playingSlot;
    private readonly ApplyEngine _applyEngine = new();

    // ── Constructor ───────────────────────────────────────────────────────
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        LoadTrackCatalog();

        _audio.PlaybackStopped += (_, _) => Dispatcher.Invoke(() =>
        {
            if (_playingSlot is not null)
            {
                _playingSlot.IsPlaying = false;
                _playingSlot = null;
            }
        });

        _applyEngine.ConflictsDetected += OnConflictsDetected;
    }

    // ── Track Catalog ─────────────────────────────────────────────────────
    private void LoadTrackCatalog()
    {
        try
        {
            var uri = new Uri("pack://application:,,,/Resources/trackCatalog.json");
            using var stream = Application.GetResourceStream(uri)!.Stream;
            var entries = JsonSerializer.Deserialize<List<CatalogEntry>>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (entries is null) return;

            foreach (var e in entries.OrderBy(e => e.Slot))
            {
                Tracks.Add(new TrackSlot { SlotNumber = e.Slot, Name = e.Name });
            }
        }
        catch (Exception ex)
        {
            AppendLog($"[ERROR] Failed to load track catalog: {ex.Message}");
        }
    }

    private record CatalogEntry(int Slot, string Name);

    // ── ROM Selection ─────────────────────────────────────────────────────
    private void SelectRom_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = "Select ALttP Randomizer ROM",
            Filter = "SNES ROM Files (*.sfc;*.smc;*.snes)|*.sfc;*.smc;*.snes|All Files (*.*)|*.*",
            CheckFileExists = true
        };

        if (dlg.ShowDialog(this) == true)
        {
            _romPath = dlg.FileName;
            RomBaseName = Path.GetFileNameWithoutExtension(dlg.FileName);
            OutputBaseName = RomBaseName;
            AppendLog($"ROM selected: {_romPath}");
        }
    }

    // ── PCM Replace / Pick ────────────────────────────────────────────────
    private void Replace_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is TrackSlot slot)
            PickPcmForSlot(slot);
    }

    private async void PickPcmForSlot(TrackSlot slot)
    {
        var dlg = new OpenFileDialog
        {
            Title = $"Select audio for Slot {slot.SlotNumber} — {slot.Name}",
            Filter = "MSU-1 PCM & Audio Files (*.pcm;*.mp3;*.wav;*.wma;*.wmv;*.aac;*.m4a;*.mp4;*.aiff;*.aif)" +
                     "|*.pcm;*.mp3;*.wav;*.wma;*.wmv;*.aac;*.m4a;*.mp4;*.aiff;*.aif" +
                     "|MSU-1 PCM (*.pcm)|*.pcm" +
                     "|MP3 (*.mp3)|*.mp3" +
                     "|WAV (*.wav)|*.wav" +
                     "|WMA / WMV (*.wma;*.wmv)|*.wma;*.wmv" +
                     "|AAC / M4A / MP4 (*.aac;*.m4a;*.mp4)|*.aac;*.m4a;*.mp4" +
                     "|AIFF (*.aiff;*.aif)|*.aiff;*.aif" +
                     "|All Files (*.*)|*.*",
            CheckFileExists = true
        };

        if (dlg.ShowDialog(this) != true) return;

        string path = dlg.FileName;
        string ext = Path.GetExtension(path).ToLowerInvariant();

        if (ext == ".pcm")
        {
            // Direct assign — validate and show
            string? error = PcmValidator.Validate(path);
            slot.PcmPath = path;
            slot.ValidationError = error;

            if (error is not null)
                AppendLog($"[WARN] Slot {slot.SlotNumber}: {error} (file: {Path.GetFileName(path)})");
            else
                AppendLog($"Slot {slot.SlotNumber} assigned: {Path.GetFileName(path)}");

            OnPropertyChanged(nameof(AssignedCountText));
            OnPropertyChanged(nameof(CanApply));
        }
        else
        {
            // Convert to MSU-1 PCM first
            string destPath = Path.ChangeExtension(path, ".pcm");
            string srcName  = Path.GetFileName(path);
            string destName = Path.GetFileName(destPath);

            AppendLog($"[Slot {slot.SlotNumber}] Converting {srcName} → {destName}...");

            _isConverting = true;
            OnPropertyChanged(nameof(CanApply));

            try
            {
                int lastMilestone = 0;
                var progress = new Progress<double>(pct =>
                {
                    int milestone = (int)(pct * 4) * 25; // 25 / 50 / 75
                    if (milestone > lastMilestone && milestone < 100)
                    {
                        AppendLog($"[Slot {slot.SlotNumber}] Converting... {milestone}%");
                        lastMilestone = milestone;
                    }
                });

                string? error = await PcmConverter.ConvertAsync(path, destPath, progress: progress);

                if (error is not null)
                {
                    AppendLog($"[ERROR] Slot {slot.SlotNumber}: {error}");
                    return;
                }

                AppendLog($"[Slot {slot.SlotNumber}] Conversion complete → {destName}");

                // Validate and assign the newly created PCM
                string? validationError = PcmValidator.Validate(destPath);
                slot.PcmPath = destPath;
                slot.ValidationError = validationError;

                if (validationError is not null)
                    AppendLog($"[WARN] Slot {slot.SlotNumber}: converted file issue: {validationError}");

                OnPropertyChanged(nameof(AssignedCountText));
                OnPropertyChanged(nameof(CanApply));
            }
            finally
            {
                _isConverting = false;
                OnPropertyChanged(nameof(CanApply));
            }
        }
    }

    // ── Clear ─────────────────────────────────────────────────────────────
    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is TrackSlot slot)
        {
            if (_playingSlot == slot)
            {
                _audio.Stop();
                slot.IsPlaying = false;
                _playingSlot = null;
            }
            slot.PcmPath = null;
            slot.ValidationError = null;
            OnPropertyChanged(nameof(AssignedCountText));
            OnPropertyChanged(nameof(CanApply));
        }
    }

    // ── Audio Preview ─────────────────────────────────────────────────────
    private void PlayStop_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is TrackSlot slot)
        {
            if (slot.IsPlaying)
            {
                _audio.Stop();
                slot.IsPlaying = false;
                _playingSlot = null;
            }
            else if (slot.PcmPath is not null)
            {
                // Stop any currently playing slot
                if (_playingSlot is not null)
                {
                    _audio.Stop();
                    _playingSlot.IsPlaying = false;
                    _playingSlot = null;
                }

                string? error = _audio.Play(slot.PcmPath);
                if (error is not null)
                {
                    AppendLog($"[ERROR] Playback failed for slot {slot.SlotNumber}: {error}");
                    MessageBox.Show(error, "Playback Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    slot.IsPlaying = true;
                    _playingSlot = slot;
                }
            }
        }
    }

    // ── Config Save / Load ────────────────────────────────────────────────
    private void SaveConfig_Click(object sender, RoutedEventArgs e)
    {
        if (_romPath is null)
        {
            ShowConfigMessage("Select a ROM first.", isError: true);
            return;
        }

        var dlg = new SaveFileDialog
        {
            Title = "Save Configuration",
            Filter = "JSON Config (*.json)|*.json|All Files (*.*)|*.*",
            DefaultExt = ".json",
            FileName = "msu-config"
        };

        if (dlg.ShowDialog(this) != true) return;

        var config = BuildConfig();
        string? error = ConfigManager.Save(dlg.FileName, config);
        if (error is null)
        {
            AppendLog($"Config saved: {dlg.FileName}");
            ShowConfigMessage("Config saved.", isError: false);
        }
        else
        {
            AppendLog($"[ERROR] {error}");
            ShowConfigMessage(error, isError: true);
        }
    }

    private void LoadConfig_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = "Load Configuration",
            Filter = "JSON Config (*.json)|*.json|All Files (*.*)|*.*",
            CheckFileExists = true
        };

        if (dlg.ShowDialog(this) != true) return;

        var (config, error) = ConfigManager.Load(dlg.FileName);
        if (error is not null)
        {
            AppendLog($"[ERROR] {error}");
            ShowConfigMessage(error, isError: true);
            return;
        }

        ApplyConfig(config!);
        AppendLog($"Config loaded: {dlg.FileName} ({config!.Tracks.Count} track(s))");
        ShowConfigMessage($"Config loaded. {config.Tracks.Count} track(s) restored.", isError: false);
    }

    private AppConfig BuildConfig()
    {
        var tracks = Tracks
            .Where(t => t.PcmPath is not null)
            .ToDictionary(t => t.SlotNumber.ToString(), t => t.PcmPath!);

        return new AppConfig
        {
            RomPath = _romPath ?? string.Empty,
            SpritePath = _spritePath ?? string.Empty,
            Tracks = tracks
        };
    }

    private void ApplyConfig(AppConfig config)
    {
        // Stop playback before changing state
        if (_playingSlot is not null)
        {
            _audio.Stop();
            _playingSlot.IsPlaying = false;
            _playingSlot = null;
        }

        // Clear all tracks
        foreach (var t in Tracks) { t.PcmPath = null; t.ValidationError = null; }

        // Set ROM
        _romPath = config.RomPath;
        RomBaseName = string.IsNullOrEmpty(config.RomPath)
            ? null
            : Path.GetFileNameWithoutExtension(config.RomPath);
        OutputBaseName = RomBaseName;

        // Set sprite
        SpritePath = string.IsNullOrEmpty(config.SpritePath) ? null : config.SpritePath;

        // Assign tracks
        foreach (var (key, pcmPath) in config.Tracks)
        {
            if (!int.TryParse(key, out int slot)) continue;
            var trackSlot = Tracks.FirstOrDefault(t => t.SlotNumber == slot);
            if (trackSlot is null) continue;

            trackSlot.PcmPath = pcmPath;
            trackSlot.ValidationError = PcmValidator.Validate(pcmPath);
        }

        OnPropertyChanged(nameof(AssignedCountText));
        OnPropertyChanged(nameof(CanApply));
        OnPropertyChanged(nameof(HasRom));
    }

    private void ShowConfigMessage(string text, bool isError)
    {
        ConfigMessage.Text = text;
        ConfigMessage.Foreground = isError
            ? new SolidColorBrush(Color.FromRgb(0xE0, 0x5C, 0x6A))
            : new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x6E));
        ConfigMessage.Visibility = Visibility.Visible;

        // Auto-hide after 4 seconds
        var timer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(4)
        };
        timer.Tick += (_, _) => { ConfigMessage.Visibility = Visibility.Collapsed; timer.Stop(); };
        timer.Start();
    }

    // ── Output Dir Browse ─────────────────────────────────────────────────
    private void BrowseOutputDir_Click(object sender, RoutedEventArgs e)
    {
        using var dlg = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "Select Output Folder for MSU Pack",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true
        };

        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            OutputDir = dlg.SelectedPath;
            AppendLog($"Output folder: {OutputDir}");
        }
    }

    // ── Apply ─────────────────────────────────────────────────────────────
    private async void Apply_Click(object sender, RoutedEventArgs e)
    {
        if (_romPath is null || _outputDir is null) return;

        // Stop playback
        if (_playingSlot is not null)
        {
            _audio.Stop();
            _playingSlot.IsPlaying = false;
            _playingSlot = null;
        }

        _isApplying = true;
        OnPropertyChanged(nameof(CanApply));
        ApplySuccessText.Visibility = Visibility.Collapsed;
        ProgressSection.Visibility = Visibility.Visible;
        ApplyProgress.Value = 0;
        ProgressStepText.Text = string.Empty;

        var tracks = Tracks
            .Where(t => t.HasFile)
            .ToDictionary(t => t.SlotNumber.ToString(), t => t.PcmPath!);

        var req = new ApplyRequest(_romPath, _outputDir, tracks, OverwriteMode.Ask, OutputBaseName?.Trim(), _spritePath);

        int lastTotal = 1;
        var progress = new Progress<(string step, int current, int total)>(p =>
        {
            lastTotal = p.total;
            ApplyProgress.Value = p.total > 0 ? (double)p.current / p.total * 100 : 0;
            ProgressStepText.Text = p.step;
        });

        try
        {
            var result = await _applyEngine.RunAsync(req, progress);

            ProgressSection.Visibility = Visibility.Collapsed;
            ApplyProgress.Value = 100;
            ApplySuccessText.Text = $"Done! {result.FilesWritten.Count} file(s) written.";
            ApplySuccessText.Visibility = Visibility.Visible;

            AppendLog($"Apply succeeded. {result.FilesWritten.Count} file(s) written to: {_outputDir}");
            foreach (var f in result.FilesWritten)
                AppendLog($"  + {Path.GetFileName(f)}");
        }
        catch (OperationCanceledException)
        {
            ProgressSection.Visibility = Visibility.Collapsed;
            AppendLog("Apply cancelled.");
        }
        catch (FileNotFoundException fnf)
        {
            ProgressSection.Visibility = Visibility.Collapsed;
            AppendLog($"[ERROR] {fnf.Message}");
            MessageBox.Show(fnf.Message, "Apply Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            ProgressSection.Visibility = Visibility.Collapsed;
            AppendLog($"[ERROR] Apply failed: {ex.Message}");
            MessageBox.Show(ex.Message, "Apply Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _isApplying = false;
            OnPropertyChanged(nameof(CanApply));
        }
    }

    // ── Conflict Modal ────────────────────────────────────────────────────
    private void OnConflictsDetected(object? sender, ConflictsDetectedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            var names = string.Join("\n", e.Conflicts.Select(c => $"  • {c.FileName}"));
            string msg = $"The following files already exist in the output folder:\n\n{names}\n\nWhat would you like to do?";

            var result = MessageBox.Show(msg, "Files Already Exist",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel,
                MessageBoxOptions.None);

            // Yes = Overwrite, No = Skip, Cancel = cancel
            e.Resolution = result switch
            {
                MessageBoxResult.Yes => OverwriteMode.Overwrite,
                MessageBoxResult.No  => OverwriteMode.Skip,
                _                    => OverwriteMode.Ask  // treated as cancel in engine
            };
            e.Complete();
        });
    }

    // ── Sprite Handlers ───────────────────────────────────────────────────
    private void BrowseSprite_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = "Select Link Sprite File",
            Filter = "Sprite Files (*.zspr;*.spr)|*.zspr;*.spr|ZSPR Sprite (*.zspr)|*.zspr|Legacy Sprite (*.spr)|*.spr|All Files (*.*)|*.*",
            CheckFileExists = true
        };

        if (dlg.ShowDialog(this) != true) return;

        string? error = SpriteApplier.Validate(dlg.FileName);
        if (error is not null)
        {
            AppendLog($"[ERROR] Invalid sprite file: {error}");
            MessageBox.Show(error, "Invalid Sprite", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        SpritePath = dlg.FileName;
        AppendLog($"Sprite selected: {Path.GetFileName(dlg.FileName)}");
    }

    private void BrowseSpritesOnline_Click(object sender, RoutedEventArgs e)
    {
        var window = new SpriteBrowserWindow { Owner = this };
        if (window.ShowDialog() != true) return;
        if (window.SelectedSpritePath is null) return;

        string? error = SpriteApplier.Validate(window.SelectedSpritePath);
        if (error is not null)
        {
            AppendLog($"[ERROR] Invalid downloaded sprite: {error}");
            return;
        }

        SpritePath = window.SelectedSpritePath;
        AppendLog($"Sprite selected: {Path.GetFileNameWithoutExtension(window.SelectedSpritePath)}");
    }

    private void ClearSprite_Click(object sender, RoutedEventArgs e)
    {
        SpritePath = null;
        AppendLog("Sprite cleared.");
    }

    // ── Log ───────────────────────────────────────────────────────────────
    private void AppendLog(string line)
    {
        Dispatcher.InvokeAsync(() =>
        {
            string ts = DateTime.Now.ToString("HH:mm:ss");
            LogBox.AppendText($"[{ts}] {line}\n");
            LogBox.ScrollToEnd();
        });
    }

    // ── INotifyPropertyChanged ────────────────────────────────────────────
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    // ── Cleanup ───────────────────────────────────────────────────────────
    protected override void OnClosed(EventArgs e)
    {
        _audio.Dispose();
        base.OnClosed(e);
    }
}
