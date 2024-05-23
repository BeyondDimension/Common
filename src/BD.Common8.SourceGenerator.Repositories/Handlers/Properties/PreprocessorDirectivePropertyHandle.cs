namespace BD.Common8.SourceGenerator.Repositories.Handlers.Properties;

/// <summary>
/// C# 预处理器指令的属性处理
/// </summary>
sealed class PreprocessorDirectivePropertyHandle : IPropertyHandle
{
    static void WriteDescription(Stream stream, string? description)
    {
        if (!string.IsNullOrEmpty(description))
        {
            stream.WriteByte(blank_space);
            stream.WriteUtf16StrToUtf8OrCustom(description!);
        }
        stream.Write(
"""



"""u8);
    }

    public bool Write(PropertyHandleArguments args)
    {
        if (args.Arguments != null)
        {
            var arguments = args.Arguments.Split(' ');
            if (!Enum.TryParse(arguments[0].Replace("#", string.Empty).Trim(), out PreprocessorDirective value))
                return false;
            var description = Regex.Replace(args.Arguments.Replace(arguments[0], ""), @"[\r\n]", "").Trim();
            switch (value)
            {
                case PreprocessorDirective.region:
                    args.Stream.Write(
"""
                    #region
                """u8);
                    WriteDescription(args.Stream, description);
                    return true;
                case PreprocessorDirective.endregion:
                    args.Stream.Write(
"""
                    #endregion
                """u8);
                    WriteDescription(args.Stream, description);
                    return true;
                case PreprocessorDirective.@if:
                    args.Stream.Write(
"""
                #if
                """u8);
                    WriteDescription(args.Stream, description);
                    return true;
                case PreprocessorDirective.elif:
                    args.Stream.Write(
"""
                #elif
                """u8);
                    WriteDescription(args.Stream, description);
                    return true;
                case PreprocessorDirective.@else:
                    args.Stream.Write(
"""
                #else
                """u8);
                    WriteDescription(args.Stream, description);
                    return true;
                case PreprocessorDirective.endif:
                    args.Stream.Write(
"""
                #endif
                """u8);
                    WriteDescription(args.Stream, description);
                    return true;
            }
        }

        return false;
    }
}
