namespace System;

public static partial class Convert2
{
    /// <summary>
    /// Converts the specified string, which encodes binary data as hex characters, to an equivalent 8-bit unsigned integer array.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>An array of 8-bit unsigned integers that is equivalent to s.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] FromHexString(string s)
#if HEXMATE
        => HexMate.Convert.FromHexString(s);
#elif NET5_0_OR_GREATER
        => System.Convert.FromHexString(s);
#else
    {
        var bytes = new byte[s.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = global::System.Convert.ToByte(s.Substring(i * 2, 2), 16);
        }
        return bytes;
    }
#endif
}