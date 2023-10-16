namespace System;

public static partial class IOPath
{
    // https://github.com/dotnet/runtime/blob/v6.0.6/src/libraries/System.Private.CoreLib/src/System/IO/Path.cs#L25-L28
    // https://github.com/dotnet/runtime/blob/v7.0.0-preview.5.22301.12/src/libraries/Common/src/System/IO/PathInternal.Unix.cs#L13-L18
    // https://github.com/dotnet/runtime/blob/v7.0.0-preview.5.22301.12/src/libraries/Common/src/System/IO/PathInternal.Windows.cs#L42-L47

    /// <summary>
    /// Unix 文件夹分隔符 char
    /// </summary>
    public const char UnixDirectorySeparatorChar = '/';

    /// <summary>
    /// Unix 文件夹分隔符 string
    /// </summary>
    public const string UnixDirectorySeparatorCharAsString = "/";

    /// <summary>
    /// Windows 文件夹分隔符 char
    /// </summary>
    public const char WinDirectorySeparatorChar = '\\';

    /// <summary>
    /// Windows 文件夹分隔符 string
    /// </summary>
    public const string WinDirectorySeparatorCharAsString = "\\";
}