using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using EFT;
using EFT.UI;
using HarmonyLib;
using TMPro;

namespace MGModClient.Services;

/// <summary>
/// 中文名称支持（预设名 / 玩家昵称）。
///
/// 背景：
///  - 枪械预设、装备预设、弹匣预设的改名窗口，以及昵称输入，都使用 <see cref="ValidationInputField"/>；
///  - 其实例上的 <c>_validationType</c> 为 <see cref="EValidationType.BuildName"/> / <see cref="EValidationType.NickName"/>，
///    白名单只含 Latin（a-zA-Z）等，不含 <c>\w</c>（.NET 的 \w 本身包含汉字）；
///  - 输入不匹配时 <c>_replacer</c> 会把非法字符直接删掉 → 表现为"中文打不进去"。
///
/// 补丁：
///  P1：<see cref="ValidationInputField.GetRegexStringFrom"/> 后置追加 CJK 范围
///      （仅对含 Latin 位的文本类字段生效，纯数字输入框不受影响）；
///  P2：<see cref="LocalizationManager.UpdateFonts"/> 后置，在"当前语言非中文"时把 SimSun
///      挂到所有 UI 字体（语言=简体中文时游戏自身已挂好，无需干预）。
///
/// 设计约束：只改内存态正则/字体 fallback，不修改任何文件与资源；异常绝不上抛。
/// </summary>
public static class ChineseNameSupport
{
    private const string CjkRange = "\\u4e00-\\u9fff";   // CJK 统一表意文字（BMP）

    private static ManualLogSource _logger;
    private static Harmony _harmony;

    /// <summary>是否放开字符校验（对应配置项 Chinese Names Enable）。</summary>
    public static bool RegexEnabled { get; set; } = true;

    /// <summary>是否强制挂中文字体 fallback（对应配置项 Chinese Names Force Font Fallback）。</summary>
    public static bool FontFallbackEnabled { get; set; } = true;

    private static void Log(string m)
    {
        if (_logger != null) _logger.LogInfo("[ChineseNames] " + m);
        else Console.WriteLine("[ChineseNames] " + m);
    }

    private static void Warn(string m)
    {
        if (_logger != null) _logger.LogWarning("[ChineseNames] " + m);
        else Console.WriteLine("[ChineseNames][WARN] " + m);
    }

    /// <summary>安装全部补丁。逐条独立，返回成功安装的数量。</summary>
    public static int Install(ManualLogSource logger)
    {
        _logger = logger;
        if (_harmony != null)
        {
            Log("补丁已安装过，跳过重复安装");
            return 0;
        }

        _harmony = new Harmony(PluginInfo.GUID + ".ChineseNames");
        int ok = 0;

        // P1：字符白名单
        try
        {
            var target = typeof(ValidationInputField).GetMethod(
                "GetRegexStringFrom",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(EValidationType) },
                null);
            var postfix = typeof(ChineseNameSupport).GetMethod(
                nameof(OnGetRegexStringFromPostfix),
                BindingFlags.NonPublic | BindingFlags.Static);
            if (target == null)
            {
                Warn("P1: 未找到 ValidationInputField.GetRegexStringFrom（可能已被移除/改名）");
            }
            else if (postfix == null)
            {
                Warn("P1: 未找到后置方法 OnGetRegexStringFromPostfix");
            }
            else
            {
                _harmony.Patch(target, postfix: new HarmonyMethod(postfix));
                Log($"P1: 安装成功 → ValidationInputField.GetRegexStringFrom({typeof(EValidationType).Name})");
                ok++;
            }
        }
        catch (Exception e)
        {
            Warn($"P1: 安装失败 {e.GetType().Name}: {e.Message}");
        }

        // P2：字体 fallback
        try
        {
            var target = typeof(LocalizationManager).GetMethod(
                nameof(LocalizationManager.UpdateFonts),
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(string) },
                null);
            var postfix = typeof(ChineseNameSupport).GetMethod(
                nameof(OnUpdateFontsPostfix),
                BindingFlags.NonPublic | BindingFlags.Static);
            if (target == null)
            {
                Warn("P2: 未找到 LocalizationManager.UpdateFonts(string)（可能已被移除/改名）");
            }
            else if (postfix == null)
            {
                Warn("P2: 未找到后置方法 OnUpdateFontsPostfix");
            }
            else
            {
                _harmony.Patch(target, postfix: new HarmonyMethod(postfix));
                Log("P2: 安装成功 → LocalizationManager.UpdateFonts(string)");
                ok++;
            }
        }
        catch (Exception e)
        {
            Warn($"P2: 安装失败 {e.GetType().Name}: {e.Message}");
        }

        Log($"补丁安装结果：{ok}/2 成功");
        return ok;
    }

    // ---------- Postfix 实现 ----------

    /// <summary>
    /// 白名单后置：仅对"文本类"字段（含 Latin 位）追加 CJK 范围。
    /// 这样 BuildName / NickName 能输中文，而纯数字（如只允许 0-9）的输入框行为不变。
    /// </summary>
    private static void OnGetRegexStringFromPostfix(EValidationType validationType, ref string __result)
    {
        if (!RegexEnabled) return;
        try
        {
            // 没有 Latin 位 = 非文本字段，保持原白名单
            if ((validationType & EValidationType.Latin) == 0) return;

            if (string.IsNullOrEmpty(__result))
            {
                __result = CjkRange;
                return;
            }

            // 幂等：避免重复追加（Init 可能对同一实例多次调用）
            if (__result.IndexOf("\\u4e00", StringComparison.Ordinal) >= 0) return;

            __result += CjkRange;
        }
        catch (Exception e)
        {
            Warn("白名单追加失败: " + e.GetType().Name + ": " + e.Message);
        }
    }

    /// <summary>
    /// 字体后置：当前语言不是中文时，把 SimSun 挂到所有 UI 字体的 fallback 表。
    /// 语言=简体中文（ch）时游戏自身的 UpdateFonts 已经处理，直接跳过。
    /// </summary>
    private static void OnUpdateFontsPostfix(string localeType)
    {
        if (!FontFallbackEnabled) return;
        try
        {
            if (string.Equals(localeType, "ch", StringComparison.OrdinalIgnoreCase)) return;

            var lm = LocalizationManager.Instance;
            if (lm == null) return;

            var languageFallbacks = lm._languageSpecificFallBacks;
            if (languageFallbacks == null ||
                !languageFallbacks.TryGetValue("ch", out var simsun) ||
                simsun == null)
            {
                return;
            }

            var fonts = lm._allFonts;
            if (fonts == null) return;

            int added = 0;
            foreach (var font in fonts)
            {
                if (font == null) continue;

                var list = font.fallbackFontAssetTable;
                if (list == null)
                {
                    list = new List<TMP_FontAsset>();
                    font.fallbackFontAssetTable = list;
                }

                if (!list.Contains(simsun))
                {
                    list.Add(simsun);
                    added++;
                }
            }

            if (added > 0)
            {
                Log($"P2: SimSun 已挂为 fallback → {added} 个字体（当前语言 {localeType}）");
            }
        }
        catch (Exception e)
        {
            Warn("字体 fallback 失败: " + e.GetType().Name + ": " + e.Message);
        }
    }
}

