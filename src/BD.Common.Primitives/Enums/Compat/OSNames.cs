namespace System.Application;

[Obsolete("use ClientOSPlatform replace OSNames.Value")]
public static class OSNames
{
    [Obsolete("use ClientOSPlatform replace OSNames.Value")]
    public enum Value : byte
    {

    }

    [Obsolete("use ClientOSPlatform replace OSNames.Value")]
    public static string ToDisplayName(this Value value)
    {
        throw new NotImplementedException();
    }

    [Obsolete("use ClientOSPlatform replace OSNames.Value")]
    public static Value Parse(string value)
    {
        throw new NotImplementedException();
    }

    [Obsolete("use ClientOSPlatform replace OSNames.Value")]
    public static bool IsAndroid(this Value value)
    {
        throw new NotImplementedException();
    }
}