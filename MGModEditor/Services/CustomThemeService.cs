// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.
using Wpf.Ui.Appearance;
using MGEditor.Models;
using Wpf.Ui.Controls;
using System.Windows;

namespace MGEditor.Services;

public static class CustomThemeService
{
    private static string? _activeKey;

    public static bool IsActive => _activeKey != null;
    public static string? ActiveKey => _activeKey;

    public static void Apply(string themeKey)
    {
        var config = CustomThemeRegistry.Get(themeKey);

        if (config == null)
            return;

        Remove();

        var baseTheme = config.Key.Contains("Dark", StringComparison.OrdinalIgnoreCase)
            ? ApplicationTheme.Dark
            : ApplicationTheme.Light;

        // 1. Load base theme without Mica backdrop
        ApplicationThemeManager.Apply(baseTheme, WindowBackdropType.None, false);

        // 2. Add custom overlay dictionary
        var overlayDict = new ResourceDictionary
        {
            Source = new Uri(
                $"pack://application:,,,/MGEditor;component/Theme/{config.DictionaryFileName}",
                UriKind.Absolute)
        };
        Application.Current.Resources.MergedDictionaries.Add(overlayDict);

        // 3. Set accent colors
        ApplicationAccentColorManager.Apply(
            config.SystemAccent,
            config.PrimaryAccent,
            config.SecondaryAccent,
            config.TertiaryAccent);

        // 4. Disable Mica and restore WPF background rendering
        if (Application.Current.MainWindow is FluentWindow fluentWindow)
        {
            if (fluentWindow.WindowBackdropType != WindowBackdropType.None)
            {
                fluentWindow.WindowBackdropType = WindowBackdropType.None;
            }

            // RemoveBackground() set a local transparent value on BackgroundProperty.
            // ClearValue lets the style's {DynamicResource WindowBackground} take effect,
            // which now resolves to our overlay's ApplicationBackgroundBrush.
            fluentWindow.ClearValue(System.Windows.Controls.Control.BackgroundProperty);
        }

        _activeKey = themeKey;
    }

    public static void Remove()
    {
        if (_activeKey == null && !IsActive)
            return;

        var toRemove = Application.Current.Resources.MergedDictionaries
            .Where(d => d.Source?.ToString()
                .Contains("MGEditor", StringComparison.OrdinalIgnoreCase) == true
                     && d.Source?.ToString()
                .Contains("/Theme/", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        foreach (var dict in toRemove)
            Application.Current.Resources.MergedDictionaries.Remove(dict);

        _activeKey = null;
    }
}

