namespace MGEditor.ControlsLookup;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class GalleryPageAttribute(string description, SymbolRegular icon, int order) : Attribute
{
    public string Description { get; } = description;

    public SymbolRegular Icon { get; } = icon;

    /// <summary>展示序号（升序）：决定首页卡片排列顺序，应与左侧导航顺序一致；新增页面必须显式标注。</summary>
    public int Order { get; } = order;
}
