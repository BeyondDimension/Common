namespace System.Collections.Generic;

/// <summary>
/// 适用于 UTF8String 忽略大小写的字符串比较
/// </summary>
public sealed class Utf8StringComparerOrdinalIgnoreCase : IEqualityComparer<byte>
{
    public Utf8StringComparerOrdinalIgnoreCase() { }

    // https://www.geeksforgeeks.org/lower-case-upper-case-interesting-fact/

    static byte Convert(byte b)
    {
        int i = b;
        i &= ~32;
        return (byte)i;
    }

    bool IEqualityComparer<byte>.Equals(byte x, byte y)
        => Convert(x) == Convert(y);

    int IEqualityComparer<byte>.GetHashCode(byte obj)
        => EqualityComparer<byte>.Default.GetHashCode(Convert(obj));
}