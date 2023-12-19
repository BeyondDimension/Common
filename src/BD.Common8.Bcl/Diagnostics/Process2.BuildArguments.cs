namespace System.Diagnostics;

partial class Process2
{
    /// <summary>
    /// 将多个参数拼接为参数字符串
    /// <para>https://github.com/dotnet/runtime/blob/v8.0.0/src/libraries/System.Diagnostics.Process/src/System/Diagnostics/ProcessStartInfo.cs#L186</para>
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(args))]
    public static string? BuildArguments(IEnumerable<string>? args)
    {
        if (args == null)
            return null;
        var arguments = new ValueStringBuilder(stackalloc char[256]);
        foreach (var arg in args)
        {
            PasteArguments.AppendArgument(ref arguments, arg);
        }
        return arguments.ToString();
    }

    /// <summary>
    /// https://github.com/dotnet/runtime/blob/v8.0.0/src/libraries/System.Private.CoreLib/src/System/PasteArguments.cs
    /// </summary>
    static partial class PasteArguments
    {
        public static void AppendArgument(ref ValueStringBuilder stringBuilder, string argument)
        {
            if (stringBuilder.Length != 0)
            {
                stringBuilder.Append(' ');
            }

            // Parsing rules for non-argv[0] arguments:
            //   - Backslash is a normal character except followed by a quote.
            //   - 2N backslashes followed by a quote ==> N literal backslashes followed by unescaped quote
            //   - 2N+1 backslashes followed by a quote ==> N literal backslashes followed by a literal quote
            //   - Parsing stops at first whitespace outside of quoted region.
            //   - (post 2008 rule): A closing quote followed by another quote ==> literal quote, and parsing remains in quoting mode.
            if (argument.Length != 0 && ContainsNoWhitespaceOrQuotes(argument))
            {
                // Simple case - no quoting or changes needed.
                stringBuilder.Append(argument);
            }
            else
            {
                stringBuilder.Append(Quote);
                int idx = 0;
                while (idx < argument.Length)
                {
                    char c = argument[idx++];
                    if (c == Backslash)
                    {
                        int numBackSlash = 1;
                        while (idx < argument.Length && argument[idx] == Backslash)
                        {
                            idx++;
                            numBackSlash++;
                        }

                        if (idx == argument.Length)
                        {
                            // We'll emit an end quote after this so must double the number of backslashes.
                            stringBuilder.Append(Backslash, numBackSlash * 2);
                        }
                        else if (argument[idx] == Quote)
                        {
                            // Backslashes will be followed by a quote. Must double the number of backslashes.
                            stringBuilder.Append(Backslash, (numBackSlash * 2) + 1);
                            stringBuilder.Append(Quote);
                            idx++;
                        }
                        else
                        {
                            // Backslash will not be followed by a quote, so emit as normal characters.
                            stringBuilder.Append(Backslash, numBackSlash);
                        }

                        continue;
                    }

                    if (c == Quote)
                    {
                        // Escape the quote so it appears as a literal. This also guarantees that we won't end up generating a closing quote followed
                        // by another quote (which parses differently pre-2008 vs. post-2008.)
                        stringBuilder.Append(Backslash);
                        stringBuilder.Append(Quote);
                        continue;
                    }

                    stringBuilder.Append(c);
                }

                stringBuilder.Append(Quote);
            }
        }

        private static bool ContainsNoWhitespaceOrQuotes(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (char.IsWhiteSpace(c) || c == Quote)
                {
                    return false;
                }
            }

            return true;
        }

        private const char Quote = '\"';
        private const char Backslash = '\\';
    }
}