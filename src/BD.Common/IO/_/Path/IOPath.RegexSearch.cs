// ReSharper disable once CheckNamespace
namespace System;

partial class IOPath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string RegexSearchFile(string filePath, string pattern)
    {
        var t = ReadAllText(filePath);
        var m = Regex.Match(t, pattern);
        return m.Success ? m.Value : "";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string RegexSearchFolder(string dirPath, string pattern, string wildcard = "")
    {
        // Foreach file in folder (until match):
        foreach (var f in Directory.GetFiles(dirPath, wildcard))
        {
            var result = RegexSearchFile(f, pattern);
            if (result == "")
                continue;
            return result;
        }

        return "";
    }
}