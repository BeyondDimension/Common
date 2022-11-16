// ReSharper disable once CheckNamespace
namespace System;

partial class IOPath
{
    // https://github.com/dotnet/runtime/blob/v6.0.6/src/libraries/System.Private.CoreLib/src/System/IO/Path.cs#L25-L28
    // https://github.com/dotnet/runtime/blob/v7.0.0-preview.5.22301.12/src/libraries/Common/src/System/IO/PathInternal.Unix.cs#L13-L18
    // https://github.com/dotnet/runtime/blob/v7.0.0-preview.5.22301.12/src/libraries/Common/src/System/IO/PathInternal.Windows.cs#L42-L47

    public const char UnixDirectorySeparatorChar = '/';
    public const string UnixDirectorySeparatorCharAsString = "/";
    public const char WinDirectorySeparatorChar = '\\';
    public const string WinDirectorySeparatorCharAsString = "\\";
}