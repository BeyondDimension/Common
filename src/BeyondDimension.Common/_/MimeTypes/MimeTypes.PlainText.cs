// https://docs.microsoft.com/en-us/dotnet/api/microsoft.jupyter.core.mimetypes?view=jupyter-dotnet

// ReSharper disable once CheckNamespace
namespace System;

public static partial class MimeTypes
{
    public const string PlainText = "text/plain";

#if DEBUG
    [Obsolete("use PlainText", true)]
    public const string Txt = PlainText;

    [Obsolete("use PlainText", true)]
    public const string Text = PlainText;
#endif
}
