// https://github.com/xamarin/Essentials/blob/1.7.3/Xamarin.Essentials/Types/ColorConverters.shared.cs

namespace System.Drawing;

/// <summary>
/// 颜色转换器助手类，提供字符串到颜色类型的转换
/// </summary>
public static partial class ColorConverters
{
    /// <summary>
    /// 将 HexStringColor 转换为 <see cref="Color"/>，如果字符串格式不正确将抛出 <see cref="ArgumentException"/>
    /// </summary>
    /// <param name="hexStringColor"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SDColor FromHex(string hexStringColor)
    {
        // Undefined
        if (hexStringColor.Length < 3)
            throw new ArgumentException(null, nameof(hexStringColor));

        var idx = (hexStringColor[0] == '#') ? 1 : 0;

        switch (hexStringColor.Length - idx)
        {
            case 3: // #rgb => ffrrggbb
                var t1 = ToHexD(hexStringColor[idx++]);
                var t2 = ToHexD(hexStringColor[idx++]);
                var t3 = ToHexD(hexStringColor[idx]);

                return Color.FromArgb((int)t1, (int)t2, (int)t3);

            case 4: // #argb => aarrggbb
                var f1 = ToHexD(hexStringColor[idx++]);
                var f2 = ToHexD(hexStringColor[idx++]);
                var f3 = ToHexD(hexStringColor[idx++]);
                var f4 = ToHexD(hexStringColor[idx]);
                return Color.FromArgb((int)f1, (int)f2, (int)f3, (int)f4);

            case 6: // #rrggbb => ffrrggbb
                return Color.FromArgb(
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx])));

            case 8: // #aarrggbb
                var a1 = ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++]);
                return Color.FromArgb(
                        (int)a1,
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx])));

            default: // everything else will result in unexpected results
                ThrowHelper.ThrowArgumentOutOfRangeException(hexStringColor);
                return default;
        }
    }

    /// <summary>
    /// 尝试将 HexStringColor 转换为 <see cref="Color"/>
    /// </summary>
    /// <param name="hexStringColor"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string hexStringColor, out Color color)
    {
        color = default;

        // Undefined
        if (hexStringColor.Length < 3)
            return false;

        var idx = (hexStringColor[0] == '#') ? 1 : 0;

        switch (hexStringColor.Length - idx)
        {
            case 3: // #rgb => ffrrggbb
                var t1 = ToHexD(hexStringColor[idx++]);
                var t2 = ToHexD(hexStringColor[idx++]);
                var t3 = ToHexD(hexStringColor[idx]);

                color = Color.FromArgb((int)t1, (int)t2, (int)t3);
                return true;

            case 4: // #argb => aarrggbb
                var f1 = ToHexD(hexStringColor[idx++]);
                var f2 = ToHexD(hexStringColor[idx++]);
                var f3 = ToHexD(hexStringColor[idx++]);
                var f4 = ToHexD(hexStringColor[idx]);
                color = Color.FromArgb((int)f1, (int)f2, (int)f3, (int)f4);
                return true;

            case 6: // #rrggbb => ffrrggbb
                color = Color.FromArgb(
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx])));
                return true;

            case 8: // #aarrggbb
                var a1 = ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++]);
                color = Color.FromArgb(
                        (int)a1,
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx])));
                return true;

            default: // everything else will result in unexpected results
                return false;
        }
    }

    /// <inheritdoc cref="ToHex"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint ToHexD(char c)
    {
        var j = ToHex(c);
        return (j << 4) | j;
    }

    /// <summary>
    /// 将字符转换为十六进制数
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint ToHex(char c)
    {
        var x = (ushort)c;
        if (x >= '0' && x <= '9')
            return (uint)(x - '0');

        x |= 0x20;
        if (x >= 'a' && x <= 'f')
            return (uint)(x - 'a' + 10);
        return 0;
    }
}
