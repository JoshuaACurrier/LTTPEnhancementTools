using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using LTTPMusicReplacer.Models;

namespace LTTPMusicReplacer;

public partial class SpriteBrowserWindow : Window
{
    // ── Static shared state ───────────────────────────────────────────────
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(30) };
    private static List<SpriteEntry>? _cachedSprites;

    private static readonly string CacheDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "LTTPMusicReplacer", "SpriteCache");

    private const string SpritesApiUrl = "https://alttpr.com/sprites";

    // ── Instance state ────────────────────────────────────────────────────
    private ICollectionView? _view;
    private string _searchText = string.Empty;

    /// <summary>Set after the user clicks "Select Sprite". Null if cancelled.</summary>
    public string? SelectedSpritePath { get; private set; }

    // ── Constructor ───────────────────────────────────────────────────────
    public SpriteBrowserWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Loading sprite list…";
        await LoadSpritesAsync();
    }

    // ── Data loading ──────────────────────────────────────────────────────
    private async Task LoadSpritesAsync()
    {
        try
        {
            if (_cachedSprites == null)
            {
                var json = await Http.GetStringAsync(SpritesApiUrl);
                var entries = JsonSerializer.Deserialize<List<SpriteEntry>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<SpriteEntry>();

                // Filter to alttpr usage (exclude smz3-only sprites, but keep those that work for both)
                _cachedSprites = entries;
            }

            // Bind to CollectionView for filtering
            _view = CollectionViewSource.GetDefaultView(_cachedSprites);
            _view.Filter = FilterSprite;
            SpriteList.ItemsSource = _view;

            LoadingText.Visibility = Visibility.Collapsed;
            SpriteList.Visibility = Visibility.Visible;
            StatusText.Text = $"{_cachedSprites.Count} sprites";
        }
        catch (Exception ex)
        {
            LoadingText.Text = $"Failed to load sprites:\n{ex.Message}";
            StatusText.Text = string.Empty;
        }
    }

    private bool FilterSprite(object obj)
    {
        if (string.IsNullOrWhiteSpace(_searchText)) return true;
        if (obj is not SpriteEntry entry) return false;

        var q = _searchText.Trim();
        if (entry.Name.Contains(q, StringComparison.OrdinalIgnoreCase)) return true;
        if (entry.Author.Contains(q, StringComparison.OrdinalIgnoreCase)) return true;
        foreach (var tag in entry.Tags)
            if (tag.Contains(q, StringComparison.OrdinalIgnoreCase)) return true;

        return false;
    }

    // ── Search ────────────────────────────────────────────────────────────
    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _searchText = SearchBox.Text;
        _view?.Refresh();

        // Update count in status
        if (_view != null)
        {
            int count = 0;
            foreach (var _ in _view) count++;
            int total = _cachedSprites?.Count ?? 0;
            StatusText.Text = count == total
                ? $"{total} sprites"
                : $"{count} / {total} sprites";
        }
    }

    // ── Selection / preview ───────────────────────────────────────────────
    private void SpriteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SpriteList.SelectedItem is SpriteEntry entry)
        {
            SelectButton.IsEnabled = true;
            ShowPreview(entry);
        }
        else
        {
            SelectButton.IsEnabled = false;
            PreviewPanel.Visibility = Visibility.Collapsed;
            PreviewPlaceholder.Visibility = Visibility.Visible;
            PreviewLoadingText.Visibility = Visibility.Collapsed;
        }
    }

    private async void ShowPreview(SpriteEntry entry)
    {
        // Show name/author immediately; load image async
        PreviewName.Text = entry.Name;
        PreviewAuthor.Text = string.IsNullOrEmpty(entry.Author) ? string.Empty : $"by {entry.Author}";

        // Build tags
        PreviewTags.Children.Clear();
        foreach (var tag in entry.Tags)
        {
            var tb = new TextBlock
            {
                Text = tag,
                Margin = new Thickness(3, 2, 3, 2),
                Padding = new Thickness(6, 2, 6, 2),
                FontSize = 10,
                Foreground = FindResource("TextMutedBrush") as System.Windows.Media.Brush
            };
            var border = new Border
            {
                Child = tb,
                Background = FindResource("SurfaceAltBrush") as System.Windows.Media.Brush,
                CornerRadius = new CornerRadius(3),
                Margin = new Thickness(2)
            };
            PreviewTags.Children.Add(border);
        }

        PreviewPlaceholder.Visibility = Visibility.Collapsed;
        PreviewImage.Source = null;

        if (!string.IsNullOrEmpty(entry.Preview))
        {
            PreviewLoadingText.Visibility = Visibility.Visible;
            PreviewPanel.Visibility = Visibility.Visible;

            try
            {
                var imageData = await Http.GetByteArrayAsync(entry.Preview);
                using var ms = new MemoryStream(imageData);
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = ms;
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                bmp.Freeze();

                // Only update if still the same selection
                if (SpriteList.SelectedItem is SpriteEntry current && current.Name == entry.Name)
                {
                    PreviewImage.Source = bmp;
                    PreviewLoadingText.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                PreviewLoadingText.Text = "Preview unavailable";
            }
        }
        else
        {
            PreviewLoadingText.Visibility = Visibility.Collapsed;
            PreviewPanel.Visibility = Visibility.Visible;
        }
    }

    // ── Select Sprite ─────────────────────────────────────────────────────
    private async void SelectSprite_Click(object sender, RoutedEventArgs e)
    {
        if (SpriteList.SelectedItem is not SpriteEntry entry) return;

        SelectButton.IsEnabled = false;
        StatusText.Text = "Downloading sprite…";

        try
        {
            Directory.CreateDirectory(CacheDir);

            // Sanitize filename
            var safeName = string.Concat(entry.Name.Split(Path.GetInvalidFileNameChars()));
            var localPath = Path.Combine(CacheDir, safeName + ".zspr");

            if (!File.Exists(localPath))
            {
                var data = await Http.GetByteArrayAsync(entry.File);
                await File.WriteAllBytesAsync(localPath, data);
            }

            SelectedSpritePath = localPath;
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Download failed: {ex.Message}";
            SelectButton.IsEnabled = true;
        }
    }

    // ── Cancel ────────────────────────────────────────────────────────────
    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
