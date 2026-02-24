using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace LTTPEnhancementTools.Models;

public class TrackSlot : INotifyPropertyChanged
{
    private string? _pcmPath;
    private string? _validationError;
    private bool _isPlaying;

    public int SlotNumber { get; init; }
    public string SlotDisplay => SlotNumber.ToString("D2");
    public string Name { get; init; } = string.Empty;

    public string? PcmPath
    {
        get => _pcmPath;
        set
        {
            if (_pcmPath != value)
            {
                _pcmPath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FileName));
                OnPropertyChanged(nameof(HasFile));
            }
        }
    }

    public string? ValidationError
    {
        get => _validationError;
        set
        {
            if (_validationError != value)
            {
                _validationError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (_isPlaying != value)
            {
                _isPlaying = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PlayButtonText));
            }
        }
    }

    public string? FileName => PcmPath is not null ? Path.GetFileName(PcmPath) : null;
    public bool HasFile => PcmPath is not null;
    public bool HasError => ValidationError is not null;
    public string PlayButtonText => IsPlaying ? "■" : "▶";

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
