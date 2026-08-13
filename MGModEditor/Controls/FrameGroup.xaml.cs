namespace MGEditor.Controls;

[ContentProperty(nameof(ExampleContent))]
public class FrameGroup: Control
{
    /// <summary>Identifies the <see cref="HeaderText"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
        nameof(HeaderText),
        typeof(string),
        typeof(FrameGroup),
        new PropertyMetadata(null)
    );
    /// <summary>Identifies the <see cref="ExampleContent"/> dependency property.</summary>
    public static readonly DependencyProperty ExampleContentProperty = DependencyProperty.Register(
        nameof(ExampleContent),
        typeof(object),
        typeof(FrameGroup),
        new PropertyMetadata(null)
    );
    public string? HeaderText
    {
        get => (string?)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }
    public object? ExampleContent
    {
        get => GetValue(ExampleContentProperty);
        set => SetValue(ExampleContentProperty, value);
    }
}
