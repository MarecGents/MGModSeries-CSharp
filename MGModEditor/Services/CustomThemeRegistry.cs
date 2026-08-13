// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Windows.Media;
using MGEditor.Models;
using MGEditor.Resources;

namespace MGEditor.Services;

/// <summary>
/// Registry of all custom themes. Add new themes here.
/// Order must match ComboBox items in SettingsPage.xaml (indices 3+).
/// </summary>
public static class CustomThemeRegistry
{
    /// <summary>
    /// All registered custom themes, in display order.
    /// </summary>
    public static readonly List<CustomThemeConfig> AllThemes = new()
    {
        new()
        {
            Key = "PurpleLight",
            DisplayNameKey = Translations.ThemePurpleLight,
            IndicatorColor = Color.FromRgb(0x9B, 0x59, 0xB6),
            DictionaryFileName = "PurpleLight.xaml",
            SystemAccent = Color.FromRgb(0x9B, 0x59, 0xB6),
            PrimaryAccent = Color.FromRgb(0x8E, 0x44, 0xAD),
            SecondaryAccent = Color.FromRgb(0x7D, 0x3C, 0x98),
            TertiaryAccent = Color.FromRgb(0x6C, 0x34, 0x83),
        },
        new()
        {
            Key = "PurpleDark",
            DisplayNameKey = Translations.ThemePurpleDark,
            IndicatorColor = Color.FromRgb(0x4A, 0x23, 0x5A),
            DictionaryFileName = "PurpleDark.xaml",
            SystemAccent = Color.FromRgb(0x9B, 0x59, 0xB6),
            PrimaryAccent = Color.FromRgb(0xAF, 0x7A, 0xC5),
            SecondaryAccent = Color.FromRgb(0xC3, 0x9B, 0xD3),
            TertiaryAccent = Color.FromRgb(0xD7, 0xBE, 0xDE),
        },
        new()
        {
            Key = "OceanLight",
            DisplayNameKey = Translations.ThemeOceanLight,
            IndicatorColor = Color.FromRgb(0x34, 0x98, 0xDB),
            DictionaryFileName = "OceanLight.xaml",
            SystemAccent = Color.FromRgb(0x34, 0x98, 0xDB),
            PrimaryAccent = Color.FromRgb(0x29, 0x80, 0xB9),
            SecondaryAccent = Color.FromRgb(0x1F, 0x6F, 0xB5),
            TertiaryAccent = Color.FromRgb(0x1A, 0x52, 0x76),
        },
        new()
        {
            Key = "OceanDark",
            DisplayNameKey = Translations.ThemeOceanDark,
            IndicatorColor = Color.FromRgb(0x0A, 0x16, 0x28),
            DictionaryFileName = "OceanDark.xaml",
            SystemAccent = Color.FromRgb(0x34, 0x98, 0xDB),
            PrimaryAccent = Color.FromRgb(0x5D, 0xAD, 0xE2),
            SecondaryAccent = Color.FromRgb(0x85, 0xC1, 0xE9),
            TertiaryAccent = Color.FromRgb(0xAE, 0xD6, 0xF1),
        },
        new()
        {
            Key = "RedLight",
            DisplayNameKey = Translations.ThemeRedLight,
            IndicatorColor = Color.FromRgb(0xE7, 0x4C, 0x3C),
            DictionaryFileName = "RedLight.xaml",
            SystemAccent = Color.FromRgb(0xE7, 0x4C, 0x3C),
            PrimaryAccent = Color.FromRgb(0xC0, 0x39, 0x2B),
            SecondaryAccent = Color.FromRgb(0xA9, 0x32, 0x26),
            TertiaryAccent = Color.FromRgb(0x92, 0x2B, 0x21)
        },
        new()
        {
            Key = "RedDark",
            DisplayNameKey = Translations.ThemeRedDark,
            IndicatorColor = Color.FromRgb(0xE7, 0x4C, 0x3C),
            DictionaryFileName = "RedDark.xaml",
            SystemAccent = Color.FromRgb(0xE7, 0x4C, 0x3C),
            PrimaryAccent = Color.FromRgb(0xEC, 0x70, 0x63),
            SecondaryAccent = Color.FromRgb(0xF1, 0x94, 0x8A),
            TertiaryAccent = Color.FromRgb(0xF5, 0xB7, 0xB1)
        },
        new()
        {
            Key = "OrangeLight",
            DisplayNameKey = Translations.ThemeOrangeLight,
            IndicatorColor = Color.FromRgb(0xF3, 0x9C, 0x12),
            DictionaryFileName = "OrangeLight.xaml",
            SystemAccent = Color.FromRgb(0xF3, 0x9C, 0x12),
            PrimaryAccent = Color.FromRgb(0xD6, 0x89, 0x10),
            SecondaryAccent = Color.FromRgb(0xCA, 0x6F, 0x1E),
            TertiaryAccent = Color.FromRgb(0xB9, 0x77, 0x0E)
        },
        new()
        {
            Key = "OrangeDark",
            DisplayNameKey = Translations.ThemeOrangeDark,
            IndicatorColor = Color.FromRgb(0xF3, 0x9C, 0x12),
            DictionaryFileName = "OrangeDark.xaml",
            SystemAccent = Color.FromRgb(0xF3, 0x9C, 0x12),
            PrimaryAccent = Color.FromRgb(0xF5, 0xB0, 0x41),
            SecondaryAccent = Color.FromRgb(0xF8, 0xC4, 0x71),
            TertiaryAccent = Color.FromRgb(0xFA, 0xD7, 0xA0)
        },
        new()
        {
            Key = "YellowLight",
            DisplayNameKey = Translations.ThemeYellowLight,
            IndicatorColor = Color.FromRgb(0xF1, 0xC4, 0x0F),
            DictionaryFileName = "YellowLight.xaml",
            SystemAccent = Color.FromRgb(0xF1, 0xC4, 0x0F),
            PrimaryAccent = Color.FromRgb(0xD4, 0xAC, 0x0D),
            SecondaryAccent = Color.FromRgb(0xB7, 0x95, 0x0B),
            TertiaryAccent = Color.FromRgb(0x9A, 0x7D, 0x0A)
        },
        new()
        {
            Key = "YellowDark",
            DisplayNameKey = Translations.ThemeYellowDark,
            IndicatorColor = Color.FromRgb(0xF1, 0xC4, 0x0F),
            DictionaryFileName = "YellowDark.xaml",
            SystemAccent = Color.FromRgb(0xF1, 0xC4, 0x0F),
            PrimaryAccent = Color.FromRgb(0xF4, 0xD0, 0x3F),
            SecondaryAccent = Color.FromRgb(0xF7, 0xDC, 0x6F),
            TertiaryAccent = Color.FromRgb(0xF9, 0xE7, 0x9F)
        },
        new()
        {
            Key = "GreenLight",
            DisplayNameKey = Translations.ThemeGreenLight,
            IndicatorColor = Color.FromRgb(0x2E, 0xCC, 0x71),
            DictionaryFileName = "GreenLight.xaml",
            SystemAccent = Color.FromRgb(0x2E, 0xCC, 0x71),
            PrimaryAccent = Color.FromRgb(0x27, 0xAE, 0x60),
            SecondaryAccent = Color.FromRgb(0x22, 0x99, 0x54),
            TertiaryAccent = Color.FromRgb(0x1E, 0x84, 0x49)
        },
        new()
        {
            Key = "GreenDark",
            DisplayNameKey = Translations.ThemeGreenDark,
            IndicatorColor = Color.FromRgb(0x2E, 0xCC, 0x71),
            DictionaryFileName = "GreenDark.xaml",
            SystemAccent = Color.FromRgb(0x2E, 0xCC, 0x71),
            PrimaryAccent = Color.FromRgb(0x58, 0xD6, 0x8D),
            SecondaryAccent = Color.FromRgb(0x82, 0xE0, 0xAA),
            TertiaryAccent = Color.FromRgb(0xAB, 0xEB, 0xC6)
        },
        new()
        {
            Key = "CyanLight",
            DisplayNameKey = Translations.ThemeCyanLight,
            IndicatorColor = Color.FromRgb(0x1A, 0xBC, 0x9C),
            DictionaryFileName = "CyanLight.xaml",
            SystemAccent = Color.FromRgb(0x1A, 0xBC, 0x9C),
            PrimaryAccent = Color.FromRgb(0x17, 0xA5, 0x89),
            SecondaryAccent = Color.FromRgb(0x14, 0x8F, 0x77),
            TertiaryAccent = Color.FromRgb(0x11, 0x7A, 0x65)
        },
        new()
        {
            Key = "CyanDark",
            DisplayNameKey = Translations.ThemeCyanDark,
            IndicatorColor = Color.FromRgb(0x1A, 0xBC, 0x9C),
            DictionaryFileName = "CyanDark.xaml",
            SystemAccent = Color.FromRgb(0x1A, 0xBC, 0x9C),
            PrimaryAccent = Color.FromRgb(0x48, 0xC9, 0xB0),
            SecondaryAccent = Color.FromRgb(0x76, 0xD7, 0xC4),
            TertiaryAccent = Color.FromRgb(0xA3, 0xE4, 0xD7)
        },
        new()
        {
            Key = "SapphireLight",
            DisplayNameKey = Translations.ThemeSapphireLight,
            IndicatorColor = Color.FromRgb(0x2E, 0x86, 0xC1),
            DictionaryFileName = "SapphireLight.xaml",
            SystemAccent = Color.FromRgb(0x2E, 0x86, 0xC1),
            PrimaryAccent = Color.FromRgb(0x28, 0x74, 0xA6),
            SecondaryAccent = Color.FromRgb(0x21, 0x61, 0x8C),
            TertiaryAccent = Color.FromRgb(0x1B, 0x4F, 0x72)
        },
        new()
        {
            Key = "SapphireDark",
            DisplayNameKey = Translations.ThemeSapphireDark,
            IndicatorColor = Color.FromRgb(0x2E, 0x86, 0xC1),
            DictionaryFileName = "SapphireDark.xaml",
            SystemAccent = Color.FromRgb(0x2E, 0x86, 0xC1),
            PrimaryAccent = Color.FromRgb(0x5D, 0xAD, 0xE2),
            SecondaryAccent = Color.FromRgb(0x85, 0xC1, 0xE9),
            TertiaryAccent = Color.FromRgb(0xAE, 0xD6, 0xF1)
        },
        new()
        {
            Key = "VioletLight",
            DisplayNameKey = Translations.ThemeVioletLight,
            IndicatorColor = Color.FromRgb(0x8E, 0x44, 0xAD),
            DictionaryFileName = "VioletLight.xaml",
            SystemAccent = Color.FromRgb(0x8E, 0x44, 0xAD),
            PrimaryAccent = Color.FromRgb(0x7D, 0x3C, 0x98),
            SecondaryAccent = Color.FromRgb(0x6C, 0x34, 0x83),
            TertiaryAccent = Color.FromRgb(0x5B, 0x2C, 0x6E)
        },
        new()
        {
            Key = "VioletDark",
            DisplayNameKey = Translations.ThemeVioletDark,
            IndicatorColor = Color.FromRgb(0x8E, 0x44, 0xAD),
            DictionaryFileName = "VioletDark.xaml",
            SystemAccent = Color.FromRgb(0x8E, 0x44, 0xAD),
            PrimaryAccent = Color.FromRgb(0xAF, 0x7A, 0xC5),
            SecondaryAccent = Color.FromRgb(0xC3, 0x9B, 0xD3),
            TertiaryAccent = Color.FromRgb(0xD7, 0xBE, 0xDE)
        },
        new()
        {
            Key = "PinkLight",
            DisplayNameKey = Translations.ThemePinkLight,
            IndicatorColor = Color.FromRgb(0xE9, 0x1E, 0x63),
            DictionaryFileName = "PinkLight.xaml",
            SystemAccent = Color.FromRgb(0xE9, 0x1E, 0x63),
            PrimaryAccent = Color.FromRgb(0xC2, 0x18, 0x5B),
            SecondaryAccent = Color.FromRgb(0xAD, 0x14, 0x57),
            TertiaryAccent = Color.FromRgb(0x88, 0x0E, 0x4F)
        },
        new()
        {
            Key = "PinkDark",
            DisplayNameKey = Translations.ThemePinkDark,
            IndicatorColor = Color.FromRgb(0xE9, 0x1E, 0x63),
            DictionaryFileName = "PinkDark.xaml",
            SystemAccent = Color.FromRgb(0xE9, 0x1E, 0x63),
            PrimaryAccent = Color.FromRgb(0xF0, 0x62, 0x92),
            SecondaryAccent = Color.FromRgb(0xF4, 0x8F, 0xB1),
            TertiaryAccent = Color.FromRgb(0xF8, 0xBB, 0xD0)
        },
        new()
        {
            Key = "BrownLight",
            DisplayNameKey = Translations.ThemeBrownLight,
            IndicatorColor = Color.FromRgb(0x8D, 0x6E, 0x63),
            DictionaryFileName = "BrownLight.xaml",
            SystemAccent = Color.FromRgb(0x8D, 0x6E, 0x63),
            PrimaryAccent = Color.FromRgb(0x7D, 0x5A, 0x50),
            SecondaryAccent = Color.FromRgb(0x6D, 0x4C, 0x41),
            TertiaryAccent = Color.FromRgb(0x5D, 0x40, 0x37)
        },
        new()
        {
            Key = "BrownDark",
            DisplayNameKey = Translations.ThemeBrownDark,
            IndicatorColor = Color.FromRgb(0x8D, 0x6E, 0x63),
            DictionaryFileName = "BrownDark.xaml",
            SystemAccent = Color.FromRgb(0x8D, 0x6E, 0x63),
            PrimaryAccent = Color.FromRgb(0xA1, 0x88, 0x7F),
            SecondaryAccent = Color.FromRgb(0xBC, 0xAA, 0xA4),
            TertiaryAccent = Color.FromRgb(0xD7, 0xCC, 0xC8)
        },
        new()
        {
            Key = "GrayLight",
            DisplayNameKey = Translations.ThemeGrayLight,
            IndicatorColor = Color.FromRgb(0x95, 0xA5, 0xA6),
            DictionaryFileName = "GrayLight.xaml",
            SystemAccent = Color.FromRgb(0x95, 0xA5, 0xA6),
            PrimaryAccent = Color.FromRgb(0x83, 0x91, 0x92),
            SecondaryAccent = Color.FromRgb(0x71, 0x7D, 0x7E),
            TertiaryAccent = Color.FromRgb(0x5F, 0x6A, 0x6A)
        },
        new()
        {
            Key = "GrayDark",
            DisplayNameKey = Translations.ThemeGrayDark,
            IndicatorColor = Color.FromRgb(0x95, 0xA5, 0xA6),
            DictionaryFileName = "GrayDark.xaml",
            SystemAccent = Color.FromRgb(0x95, 0xA5, 0xA6),
            PrimaryAccent = Color.FromRgb(0xAB, 0xB2, 0xB3),
            SecondaryAccent = Color.FromRgb(0xBF, 0xC9, 0xCA),
            TertiaryAccent = Color.FromRgb(0xD5, 0xDB, 0xDB)
        },
        new()
        {
            Key = "BlackLight",
            DisplayNameKey = Translations.ThemeBlackLight,
            IndicatorColor = Color.FromRgb(0x2C, 0x3E, 0x50),
            DictionaryFileName = "BlackLight.xaml",
            SystemAccent = Color.FromRgb(0x2C, 0x3E, 0x50),
            PrimaryAccent = Color.FromRgb(0x21, 0x2F, 0x3D),
            SecondaryAccent = Color.FromRgb(0x1B, 0x26, 0x31),
            TertiaryAccent = Color.FromRgb(0x17, 0x20, 0x2A)
        },
        new()
        {
            Key = "BlackDark",
            DisplayNameKey = Translations.ThemeBlackDark,
            IndicatorColor = Color.FromRgb(0x2C, 0x3E, 0x50),
            DictionaryFileName = "BlackDark.xaml",
            SystemAccent = Color.FromRgb(0x2C, 0x3E, 0x50),
            PrimaryAccent = Color.FromRgb(0x5D, 0x6D, 0x7E),
            SecondaryAccent = Color.FromRgb(0x85, 0x92, 0x9E),
            TertiaryAccent = Color.FromRgb(0xAE, 0xB6, 0xBF)
        },
        new()
        {
            Key = "WhiteLight",
            DisplayNameKey = Translations.ThemeWhiteLight,
            IndicatorColor = Color.FromRgb(0xEC, 0xF0, 0xF1),
            DictionaryFileName = "WhiteLight.xaml",
            SystemAccent = Color.FromRgb(0xEC, 0xF0, 0xF1),
            PrimaryAccent = Color.FromRgb(0xD5, 0xDB, 0xDB),
            SecondaryAccent = Color.FromRgb(0xBF, 0xC9, 0xCA),
            TertiaryAccent = Color.FromRgb(0xAB, 0xB2, 0xB3)
        },
        new()
        {
            Key = "WhiteDark",
            DisplayNameKey = Translations.ThemeWhiteDark,
            IndicatorColor = Color.FromRgb(0xEC, 0xF0, 0xF1),
            DictionaryFileName = "WhiteDark.xaml",
            SystemAccent = Color.FromRgb(0xEC, 0xF0, 0xF1),
            PrimaryAccent = Color.FromRgb(0xF0, 0xF3, 0xF4),
            SecondaryAccent = Color.FromRgb(0xF7, 0xF9, 0xF9),
            TertiaryAccent = Color.FromRgb(0xFF, 0xFF, 0xFF)
        },
        new()
        {
            Key = "MagentaLight",
            DisplayNameKey = Translations.ThemeMagentaLight,
            IndicatorColor = Color.FromRgb(0xC2, 0x18, 0x5B),
            DictionaryFileName = "MagentaLight.xaml",
            SystemAccent = Color.FromRgb(0xC2, 0x18, 0x5B),
            PrimaryAccent = Color.FromRgb(0xAD, 0x14, 0x57),
            SecondaryAccent = Color.FromRgb(0x88, 0x0E, 0x4F),
            TertiaryAccent = Color.FromRgb(0x6A, 0x0D, 0x3B)
        },
        new()
        {
            Key = "MagentaDark",
            DisplayNameKey = Translations.ThemeMagentaDark,
            IndicatorColor = Color.FromRgb(0xC2, 0x18, 0x5B),
            DictionaryFileName = "MagentaDark.xaml",
            SystemAccent = Color.FromRgb(0xC2, 0x18, 0x5B),
            PrimaryAccent = Color.FromRgb(0xD6, 0x33, 0x84),
            SecondaryAccent = Color.FromRgb(0xE9, 0x1E, 0x8C),
            TertiaryAccent = Color.FromRgb(0xF0, 0x62, 0x92)
        },
        new()
        {
            Key = "LemonLight",
            DisplayNameKey = Translations.ThemeLemonLight,
            IndicatorColor = Color.FromRgb(0xFF, 0xF1, 0x76),
            DictionaryFileName = "LemonLight.xaml",
            SystemAccent = Color.FromRgb(0xFF, 0xF1, 0x76),
            PrimaryAccent = Color.FromRgb(0xFD, 0xD8, 0x35),
            SecondaryAccent = Color.FromRgb(0xFB, 0xC0, 0x2D),
            TertiaryAccent = Color.FromRgb(0xF9, 0xA8, 0x25)
        },
        new()
        {
            Key = "LemonDark",
            DisplayNameKey = Translations.ThemeLemonDark,
            IndicatorColor = Color.FromRgb(0xFF, 0xF1, 0x76),
            DictionaryFileName = "LemonDark.xaml",
            SystemAccent = Color.FromRgb(0xFF, 0xF1, 0x76),
            PrimaryAccent = Color.FromRgb(0xFF, 0xF5, 0x9D),
            SecondaryAccent = Color.FromRgb(0xFF, 0xF9, 0xC4),
            TertiaryAccent = Color.FromRgb(0xFF, 0xFD, 0xE7)
        },
        new()
        {
            Key = "IndigoLight",
            DisplayNameKey = Translations.ThemeIndigoLight,
            IndicatorColor = Color.FromRgb(0x39, 0x49, 0xAB),
            DictionaryFileName = "IndigoLight.xaml",
            SystemAccent = Color.FromRgb(0x39, 0x49, 0xAB),
            PrimaryAccent = Color.FromRgb(0x30, 0x3F, 0x9F),
            SecondaryAccent = Color.FromRgb(0x28, 0x35, 0x93),
            TertiaryAccent = Color.FromRgb(0x1A, 0x23, 0x7E)
        },
        new()
        {
            Key = "IndigoDark",
            DisplayNameKey = Translations.ThemeIndigoDark,
            IndicatorColor = Color.FromRgb(0x39, 0x49, 0xAB),
            DictionaryFileName = "IndigoDark.xaml",
            SystemAccent = Color.FromRgb(0x39, 0x49, 0xAB),
            PrimaryAccent = Color.FromRgb(0x5C, 0x6B, 0xC0),
            SecondaryAccent = Color.FromRgb(0x79, 0x86, 0xCB),
            TertiaryAccent = Color.FromRgb(0x9F, 0xA8, 0xDA)
        },
        new()
        {
            Key = "TeaLight",
            DisplayNameKey = Translations.ThemeTeaLight,
            IndicatorColor = Color.FromRgb(0xA1, 0x88, 0x7F),
            DictionaryFileName = "TeaLight.xaml",
            SystemAccent = Color.FromRgb(0xA1, 0x88, 0x7F),
            PrimaryAccent = Color.FromRgb(0x8D, 0x6E, 0x63),
            SecondaryAccent = Color.FromRgb(0x7D, 0x5A, 0x50),
            TertiaryAccent = Color.FromRgb(0x6D, 0x4C, 0x41)
        },
        new()
        {
            Key = "TeaDark",
            DisplayNameKey = Translations.ThemeTeaDark,
            IndicatorColor = Color.FromRgb(0xA1, 0x88, 0x7F),
            DictionaryFileName = "TeaDark.xaml",
            SystemAccent = Color.FromRgb(0xA1, 0x88, 0x7F),
            PrimaryAccent = Color.FromRgb(0xBC, 0xAA, 0xA4),
            SecondaryAccent = Color.FromRgb(0xD7, 0xCC, 0xC8),
            TertiaryAccent = Color.FromRgb(0xEB, 0xE1, 0xDC)
        },
    };

    /// <summary>Gets a theme config by key.</summary>
    public static CustomThemeConfig? Get(string key) =>
        AllThemes.FirstOrDefault(t => t.Key == key);

    /// <summary>Gets the index of a theme key in the registry.</summary>
    public static int IndexOf(string key)
    {
        for (int i = 0; i < AllThemes.Count; i++)
        {
            if (AllThemes[i].Key == key)
                return i;
        }

        return -1;
    }
}

