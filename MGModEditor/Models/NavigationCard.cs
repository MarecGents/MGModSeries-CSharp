using System.ComponentModel;

namespace MGEditor.Models;

public record NavigationCard : INotifyPropertyChanged
{
    public string? Name { get; init; }

    public SymbolRegular Icon { get; init; }

    public string? Description { get; init; }

    public Type? PageType { get; init; }

    /// <summary>每张卡片高度（由 GalleryNavigationPresenter 测量后写入，语言切换时重建新实例重新计算）。</summary>
    private double _cardHeight;
    public double CardHeight
    {
        get => _cardHeight;
        set
        {
            if (_cardHeight != value)
            {
                _cardHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CardHeight)));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
