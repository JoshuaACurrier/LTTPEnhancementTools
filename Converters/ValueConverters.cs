using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;

namespace LTTPEnhancementTools.Converters;

/// <summary>Converts null/empty to Visibility.Collapsed, non-null to Visible.</summary>
public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value is string s ? !string.IsNullOrEmpty(s) : value != null)
            ? Visibility.Visible
            : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Inverts NullToVisibility — shows when null/empty.</summary>
public class NullToVisibilityInverseConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value is string s ? !string.IsNullOrEmpty(s) : value != null)
            ? Visibility.Collapsed
            : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Returns the danger brush when value is non-null/non-empty (validation error present).</summary>
public class ErrorToBrushConverter : IValueConverter
{
    public Brush ErrorBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0xE0, 0x5C, 0x6A));
    public Brush NormalBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0xB8, 0xB8, 0xC8));

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value is string s && !string.IsNullOrEmpty(s)) ? ErrorBrush : NormalBrush;

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Bool to Visibility.</summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Bool to Visibility (inverted).</summary>
public class BoolToVisibilityInverseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Converts a URL string to a BitmapImage using WPF's built-in async HTTP download.
/// Returns null for null/empty strings.
/// </summary>
public class UrlToImageSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string url || string.IsNullOrEmpty(url)) return null;
        try
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(url, UriKind.Absolute);
            bmp.DecodePixelWidth = 64; // decode at thumbnail size to save memory
            bmp.EndInit();
            return bmp;
        }
        catch
        {
            return null;
        }
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
