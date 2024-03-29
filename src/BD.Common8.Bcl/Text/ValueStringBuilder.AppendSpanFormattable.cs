// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/runtime/blob/v8.0.0/src/libraries/Common/src/System/Text/ValueStringBuilder.AppendSpanFormattable.cs

namespace System.Text;

ref partial struct ValueStringBuilder
{
    public void AppendSpanFormattable<T>(T value, string? format = null, IFormatProvider? provider = null) where T : ISpanFormattable
    {
        if (value.TryFormat(_chars[_pos..], out int charsWritten, format, provider))
        {
            _pos += charsWritten;
        }
        else
        {
            Append(value.ToString(format, provider));
        }
    }
}