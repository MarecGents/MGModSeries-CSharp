namespace MGEditor.Controls;

public class CheckButtonFrame : Control
{
    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register(
            nameof(IsChecked),
            typeof(bool?),
            typeof(CheckButtonFrame),
            new FrameworkPropertyMetadata(
                false, 
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    public static readonly DependencyProperty ButtonContentProperty =
        DependencyProperty.Register(
            nameof(ButtonContent),
            typeof(string),
            typeof(CheckButtonFrame),
            new PropertyMetadata("Default Content")
        );

    public bool? IsChecked
    {
        get => (bool?)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public string? ButtonContent
    {
        get => (string?)GetValue(ButtonContentProperty);
        set => SetValue(ButtonContentProperty, value);
    }


    public ICommand ToggleCheckCommand { get; }

    public CheckButtonFrame()
    {
        ToggleCheckCommand = new RelayCommand(() =>
        {
            IsChecked = !(IsChecked ?? false);
        });

    }

}
