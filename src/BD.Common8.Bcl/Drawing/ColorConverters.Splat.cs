using Splat;

namespace System.Drawing;

public static partial class ColorConverters
{
    /// <summary>
    /// 尝试将 HexStringColor 转换为 <see cref="SplatColor"/>
    /// </summary>
    /// <param name="hexStringColor"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string hexStringColor, out SplatColor color)
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

                color = SplatColor.FromArgb((int)t1, (int)t2, (int)t3);
                return true;

            case 4: // #argb => aarrggbb
                var f1 = ToHexD(hexStringColor[idx++]);
                var f2 = ToHexD(hexStringColor[idx++]);
                var f3 = ToHexD(hexStringColor[idx++]);
                var f4 = ToHexD(hexStringColor[idx]);
                color = SplatColor.FromArgb((int)f1, (int)f2, (int)f3, (int)f4);
                return true;

            case 6: // #rrggbb => ffrrggbb
                color = SplatColor.FromArgb(
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx])));
                return true;

            case 8: // #aarrggbb
                var a1 = ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++]);
                color = SplatColor.FromArgb(
                        (int)a1,
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx])));
                return true;

            default: // everything else will result in unexpected results
                return false;
        }
    }

    /// <summary>
    /// 将 HexStringColor 转换为 <see cref="SplatColor"/>，如果字符串格式不正确将抛出 <see cref="ArgumentException"/>
    /// </summary>
    /// <param name="hexStringColor"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplatColor FromHexToSplatColor(string hexStringColor)
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

                return SplatColor.FromArgb((int)t1, (int)t2, (int)t3);

            case 4: // #argb => aarrggbb
                var f1 = ToHexD(hexStringColor[idx++]);
                var f2 = ToHexD(hexStringColor[idx++]);
                var f3 = ToHexD(hexStringColor[idx++]);
                var f4 = ToHexD(hexStringColor[idx]);
                return SplatColor.FromArgb((int)f1, (int)f2, (int)f3, (int)f4);

            case 6: // #rrggbb => ffrrggbb
                return SplatColor.FromArgb(
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx])));

            case 8: // #aarrggbb
                var a1 = ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++]);
                return SplatColor.FromArgb(
                        (int)a1,
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx++])),
                        (int)(ToHex(hexStringColor[idx++]) << 4 | ToHex(hexStringColor[idx])));

            default: // everything else will result in unexpected results
                ThrowHelper.ThrowArgumentOutOfRangeException(hexStringColor);
                return default;
        }
    }
}