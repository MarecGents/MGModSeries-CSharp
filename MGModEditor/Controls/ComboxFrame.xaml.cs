namespace MGEditor.Controls;


public class ComboxFrame : Control
{
    public static readonly DependencyProperty ButtonContentProperty = 
        DependencyProperty.Register(
            nameof(ButtonContent),
            typeof(string),
            typeof(ComboxFrame),
            new PropertyMetadata("Default Content")
        );

    public static readonly DependencyProperty ItemsSourceProperty = 
        DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(object),
            typeof(ComboxFrame),
            new PropertyMetadata(null)
        );

    public static readonly DependencyProperty SelectedValueProperty =
        DependencyProperty.Register(
            nameof(SelectedValue),
            typeof(object),
            typeof(ComboxFrame),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

    public string? ButtonContent
    {
        get => (string?)GetValue(ButtonContentProperty);
        set => SetValue(ButtonContentProperty, value);
    }

    public object? ItemsSource
    {
        get => (object?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
    public object? SelectedValue
    {
        get => (object?)GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }


    public ComboxFrame()
    {
        
    }


}
