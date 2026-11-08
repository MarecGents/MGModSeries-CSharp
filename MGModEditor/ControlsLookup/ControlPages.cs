namespace MGEditor.ControlsLookup;

internal static class ControlPages
{
    private const string PageSuffix = "Page";

    public static IEnumerable<GalleryPage> All()
    {
        // 顺序：显式 Order 升序；同号时以类名兜底，保证结果稳定（新增页面须在 [GalleryPage] 标注序号）
        foreach (
            Type? type in GalleryAssembly
                .Asssembly.GetTypes()
                .Where(t => t.IsDefined(typeof(GalleryPageAttribute)))
                .OrderBy(t => t.GetCustomAttributes<GalleryPageAttribute>().FirstOrDefault()?.Order ?? int.MaxValue)
                .ThenBy(t => t.Name, StringComparer.Ordinal)
        )
        {
            GalleryPageAttribute? galleryPageAttribute = type.GetCustomAttributes<GalleryPageAttribute>()
                .FirstOrDefault();

            if (galleryPageAttribute is not null)
            {
                yield return new GalleryPage(
                    type.Name[..type.Name.LastIndexOf(PageSuffix)],
                    galleryPageAttribute.Description,
                    galleryPageAttribute.Icon,
                    type
                );
            }
        }
    }

    public static IEnumerable<GalleryPage> FromNamespace(string namespaceName)
    {
        return All().Where(t => t.PageType?.Namespace?.StartsWith(namespaceName) ?? false);
    }
}
