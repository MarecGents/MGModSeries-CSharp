namespace MGEditor.Models;

public class AppSetting
{
    public AppPersonalized Personalized  { get; set; }
}

public class AppPersonalized
{
    public string Theme { get; set; }
    public string? Language { get; set; }
}