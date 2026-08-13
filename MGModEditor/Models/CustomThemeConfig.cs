// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

namespace MGEditor.Models;

/// <summary>
/// Configuration for a single custom theme entry.
/// </summary>
public sealed record CustomThemeConfig
{
    /// <summary>Unique identifier, e.g. "PurpleLight".</summary>
    public required string Key { get; init; }

    /// <summary>Translation key of the display name shown in ComboBox.</summary>
    public required string DisplayNameKey { get; init; }

    /// <summary>Color of the indicator dot in ComboBox.</summary>
    public required System.Windows.Media.Color IndicatorColor { get; init; }

    /// <summary>XAML dictionary file name (e.g. "PurpleLight.xaml").</summary>
    public required string DictionaryFileName { get; init; }

    /// <summary>Primary accent color (SystemAccentColor).</summary>
    public required System.Windows.Media.Color SystemAccent { get; init; }

    /// <summary>Primary variant accent (AccentFillColorDefault).</summary>
    public required System.Windows.Media.Color PrimaryAccent { get; init; }

    /// <summary>Secondary variant accent.</summary>
    public required System.Windows.Media.Color SecondaryAccent { get; init; }

    /// <summary>Tertiary variant accent.</summary>
    public required System.Windows.Media.Color TertiaryAccent { get; init; }
}
