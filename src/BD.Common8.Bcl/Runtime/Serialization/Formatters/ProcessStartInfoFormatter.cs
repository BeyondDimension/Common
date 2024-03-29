namespace System.Runtime.Serialization.Formatters;

/// <summary>
/// 对类型 <see cref="ProcessStartInfo"/>, <see cref="ProcessStartInfoPackable"/> 的序列化与反序列化实现
/// </summary>
public sealed class ProcessStartInfoFormatter :
    IMemoryPackFormatter<ProcessStartInfo?>,
    IMemoryPackFormatter<ProcessStartInfoPackable?>,
    IMemoryPackFormatter<ProcessStartInfoPackable>
{
    /// <summary>
    /// 默认的 <see cref="ProcessStartInfoFormatter"/> 实例
    /// </summary>
    public static readonly ProcessStartInfoFormatter Default = new();

    /// <inheritdoc/>
    void IMemoryPackFormatter<ProcessStartInfo?>.Deserialize(ref MemoryPackReader reader, scoped ref ProcessStartInfo? value)
    {
        if (reader.PeekIsNull())
        {
            value = null;
        }
        else
        {
            value = reader.ReadPackable<ProcessStartInfoPackable>();
        }
    }

    /// <inheritdoc/>
    void IMemoryPackFormatter<ProcessStartInfoPackable>.Deserialize(ref MemoryPackReader reader, scoped ref ProcessStartInfoPackable value)
    {
        if (reader.PeekIsNull())
        {
            value = default;
        }
        else
        {
            value = reader.ReadPackable<ProcessStartInfoPackable>();
        }
    }

    /// <inheritdoc/>
    void IMemoryPackFormatter<ProcessStartInfoPackable?>.Deserialize(ref MemoryPackReader reader, scoped ref ProcessStartInfoPackable? value)
    {
        if (reader.PeekIsNull())
        {
            value = default;
        }
        else
        {
            value = reader.ReadPackable<ProcessStartInfoPackable>();
        }
    }

    /// <inheritdoc/>
    void IMemoryPackFormatter<ProcessStartInfo?>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref ProcessStartInfo? value)
    {
        if (value == null)
        {
            writer.WriteNullObjectHeader();
        }
        else
        {
            ProcessStartInfoPackable packable = value;
            writer.WritePackable(packable);
        }
    }

    /// <inheritdoc/>
    void IMemoryPackFormatter<ProcessStartInfoPackable>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref ProcessStartInfoPackable value)
    {
        if (value == default)
        {
            writer.WriteNullObjectHeader();
        }
        else
        {
            writer.WritePackable(value);
        }
    }

    /// <inheritdoc/>
    void IMemoryPackFormatter<ProcessStartInfoPackable?>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref ProcessStartInfoPackable? value)
    {
        if (!value.HasValue || value.Value == default)
        {
            writer.WriteNullObjectHeader();
        }
        else
        {
            writer.WritePackable(value.Value);
        }
    }
}

[MemoryPackable]
public readonly partial record struct ProcessStartInfoPackable
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ProcessStartInfo?(ProcessStartInfoPackable value)
    {
        if (value == default) return default;
        static Encoding? GetEncoding(int? codepage) => codepage.HasValue ? Encoding.GetEncoding(codepage.Value) : null;
        ProcessStartInfo psi = new()
        {
            Verb = value.Verb,
            UseShellExecute = value.UseShellExecute,
            UserName = value.UserName,
            StandardErrorEncoding = GetEncoding(value.StandardErrorEncoding),
            StandardInputEncoding = GetEncoding(value.StandardInputEncoding),
            StandardOutputEncoding = GetEncoding(value.StandardOutputEncoding),
            RedirectStandardError = value.RedirectStandardError,
            RedirectStandardInput = value.RedirectStandardInput,
            RedirectStandardOutput = value.RedirectStandardOutput,
            FileName = value.FileName,
            ErrorDialogParentHandle = value.ErrorDialogParentHandle,
            ErrorDialog = value.ErrorDialog,
            CreateNoWindow = value.CreateNoWindow,
            WindowStyle = value.WindowStyle,
            WorkingDirectory = value.WorkingDirectory,
        };
        if (OperatingSystem.IsWindows())
        {
            psi.LoadUserProfile = value.LoadUserProfile;
            psi.PasswordInClearText = value.PasswordInClearText;
            psi.Domain = value.Domain;
        }
        if (value.ArgumentList.Length != 0)
        {
            for (int i = 0; i < value.ArgumentList.Length; i++)
            {
                psi.ArgumentList.Add(value.ArgumentList[i]);
            }
        }
        else if (!string.IsNullOrEmpty(value.Arguments))
        {
            psi.Arguments = value.Arguments;
        }
        var environment = value.Environment;
        if (environment != default)
        {
            foreach (var item in environment)
            {
                if (psi.Environment.ContainsKey(item.Key))
                {
                    psi.Environment[item.Key] = item.Value;
                }
                else
                {
                    psi.Environment.Add(item);
                }
            }
        }
        return psi;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ProcessStartInfoPackable(ProcessStartInfo value)
        => new(
            value.Verb,
            value.UseShellExecute,
            value.UserName,
            value.StandardOutputEncoding?.CodePage,
            value.StandardInputEncoding?.CodePage,
            value.StandardErrorEncoding?.CodePage,
            value.RedirectStandardOutput,
            value.RedirectStandardInput,
            value.RedirectStandardError,
            OperatingSystem.IsWindows() ? value.PasswordInClearText : default,
            OperatingSystem.IsWindows() && value.LoadUserProfile,
            value.FileName,
            value.ErrorDialogParentHandle,
            value.ErrorDialog,
            value.Environment.ToImmutableDictionary(),
            OperatingSystem.IsWindows() ? value.Domain : default,
            value.CreateNoWindow,
            value.Arguments,
            [.. value.ArgumentList],
            value.WindowStyle,
            value.WorkingDirectory);

    [MemoryPackConstructor]
    ProcessStartInfoPackable(
        string? verb,
        bool useShellExecute,
        string? userName,
        int? standardOutputEncoding,
        int? standardInputEncoding,
        int? standardErrorEncoding,
        bool redirectStandardOutput,
        bool redirectStandardInput,
        bool redirectStandardError,
        string? passwordInClearText,
        bool loadUserProfile,
        string? fileName,
        nint errorDialogParentHandle,
        bool errorDialog,
        ImmutableDictionary<string, string?> environment,
        string? domain,
        bool createNoWindow,
        string? arguments,
        ImmutableArray<string> argumentList,
        ProcessWindowStyle windowStyle,
        string? workingDirectory)
    {
        if (!string.IsNullOrEmpty(verb))
            Verb = verb;
        UseShellExecute = useShellExecute;
        if (!string.IsNullOrEmpty(userName))
            UserName = userName;
        StandardOutputEncoding = standardOutputEncoding;
        StandardInputEncoding = standardInputEncoding;
        StandardErrorEncoding = standardErrorEncoding;
        RedirectStandardOutput = redirectStandardOutput;
        RedirectStandardInput = redirectStandardInput;
        RedirectStandardError = redirectStandardError;
        if (!string.IsNullOrEmpty(passwordInClearText))
            PasswordInClearText = passwordInClearText;
        LoadUserProfile = loadUserProfile;
        if (!string.IsNullOrEmpty(fileName))
            FileName = fileName;
        ErrorDialogParentHandle = errorDialogParentHandle;
        ErrorDialog = errorDialog;
        Environment = environment;
        if (!string.IsNullOrEmpty(domain))
            Domain = domain;
        CreateNoWindow = createNoWindow;
        if (!string.IsNullOrEmpty(arguments))
            Arguments = arguments;
        ArgumentList = argumentList;
        WindowStyle = windowStyle;
        if (!string.IsNullOrEmpty(workingDirectory))
            WorkingDirectory = workingDirectory;
    }

    /// <inheritdoc cref="ProcessStartInfo.Verb"/>
    [MemoryPackInclude]
    string? Verb { get; }

    /// <inheritdoc cref="ProcessStartInfo.UseShellExecute"/>
    [MemoryPackInclude]
    bool UseShellExecute { get; }

    /// <inheritdoc cref="ProcessStartInfo.UserName"/>
    [MemoryPackInclude]
    string? UserName { get; }

    /// <inheritdoc cref="ProcessStartInfo.StandardOutputEncoding"/>
    [MemoryPackInclude]
    int? StandardOutputEncoding { get; }

    /// <inheritdoc cref="ProcessStartInfo.StandardInputEncoding"/>
    [MemoryPackInclude]
    int? StandardInputEncoding { get; }

    /// <inheritdoc cref="ProcessStartInfo.StandardErrorEncoding"/>
    [MemoryPackInclude]
    int? StandardErrorEncoding { get; }

    /// <inheritdoc cref="ProcessStartInfo.RedirectStandardOutput"/>
    [MemoryPackInclude]
    bool RedirectStandardOutput { get; }

    /// <inheritdoc cref="ProcessStartInfo.RedirectStandardInput"/>
    [MemoryPackInclude]
    bool RedirectStandardInput { get; }

    /// <inheritdoc cref="ProcessStartInfo.RedirectStandardError"/>
    [MemoryPackInclude]
    bool RedirectStandardError { get; }

    /// <inheritdoc cref="ProcessStartInfo.PasswordInClearText"/>
    [MemoryPackInclude]
    string? PasswordInClearText { get; }

    /// <inheritdoc cref="ProcessStartInfo.LoadUserProfile"/>
    [MemoryPackInclude]
    bool LoadUserProfile { get; }

    /// <inheritdoc cref="ProcessStartInfo.FileName"/>
    [MemoryPackInclude]
    string? FileName { get; }

    /// <inheritdoc cref="ProcessStartInfo.ErrorDialogParentHandle"/>
    [MemoryPackInclude]
    nint ErrorDialogParentHandle { get; }

    /// <inheritdoc cref="ProcessStartInfo.ErrorDialog"/>
    [MemoryPackInclude]
    bool ErrorDialog { get; }

    /// <inheritdoc cref="ProcessStartInfo.Environment"/>
    [MemoryPackInclude]
    ImmutableDictionary<string, string?> Environment { get; }

    /// <inheritdoc cref="ProcessStartInfo.Domain"/>
    [MemoryPackInclude]
    string? Domain { get; }

    /// <inheritdoc cref="ProcessStartInfo.CreateNoWindow"/>
    [MemoryPackInclude]
    bool CreateNoWindow { get; }

    /// <inheritdoc cref="ProcessStartInfo.Arguments"/>
    [MemoryPackInclude]
    string? Arguments { get; }

    /// <inheritdoc cref="ProcessStartInfo.ArgumentList"/>
    [MemoryPackInclude]
    ImmutableArray<string> ArgumentList { get; }

    /// <inheritdoc cref="ProcessStartInfo.WindowStyle"/>
    [MemoryPackInclude]
    ProcessWindowStyle WindowStyle { get; }

    /// <inheritdoc cref="ProcessStartInfo.WorkingDirectory"/>
    [MemoryPackInclude]
    string? WorkingDirectory { get; }
}

/// <summary>
/// 可空 <see cref="ProcessStartInfo"/> 格式化程序特性
/// </summary>
public sealed class ProcessStartInfoFormatterAttribute : MemoryPackCustomFormatterAttribute<ProcessStartInfoFormatter, ProcessStartInfo?>
{
    /// <summary>
    /// 获取 <see cref="ProcessStartInfoFormatter.Default"/> 实例
    /// </summary>
    public sealed override ProcessStartInfoFormatter GetFormatter() => ProcessStartInfoFormatter.Default;

    /// <summary>
    /// 可空 <see cref="ProcessStartInfo"/> 格式化程序
    /// </summary>
    public sealed class Formatter : MemoryPackFormatter<ProcessStartInfo?>
    {
        /// <summary>
        /// 默认的 <see cref="Formatter"/> 实例
        /// </summary>
        public static readonly Formatter Default = new();

        /// <summary>
        /// 将可空 <see cref="ProcessStartInfo"/> 对象序列化到 <see cref="MemoryPackWriter{TBufferWriter}"/> 对象
        /// </summary>
        /// <typeparam name="TBufferWriter"></typeparam>
        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref ProcessStartInfo? value)
        {
            IMemoryPackFormatter<ProcessStartInfo?> f = ProcessStartInfoFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        /// <summary>
        /// 从 <see cref="MemoryPackReader"/> 对象反序列化为可空 <see cref="ProcessStartInfo"/> 对象
        /// </summary>
        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref ProcessStartInfo? value)
        {
            IMemoryPackFormatter<ProcessStartInfo?> f = ProcessStartInfoFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

/// <summary>
/// 可空 <see cref="ProcessStartInfoPackable"/> 格式化程序特性
/// </summary>
public sealed class ProcessStartInfoPackableNullableFormatterAttribute : MemoryPackCustomFormatterAttribute<ProcessStartInfoFormatter, ProcessStartInfoPackable?>
{
    /// <summary>
    /// 获取 <see cref="ProcessStartInfoFormatter.Default"/> 实例
    /// </summary>
    public sealed override ProcessStartInfoFormatter GetFormatter() => ProcessStartInfoFormatter.Default;

    /// <summary>
    /// 可空 <see cref="ProcessStartInfoPackable"/> 格式化程序
    /// </summary>
    public sealed class Formatter : MemoryPackFormatter<ProcessStartInfoPackable?>
    {
        /// <summary>
        /// 默认的 <see cref="Formatter"/> 实例
        /// </summary>
        public static readonly Formatter Default = new();

        /// <summary>
        /// 将可空 <see cref="ProcessStartInfoPackable"/> 对象序列化到 <see cref="MemoryPackWriter{TBufferWriter}"/> 对象
        /// </summary>
        /// <typeparam name="TBufferWriter"></typeparam>
        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref ProcessStartInfoPackable? value)
        {
            IMemoryPackFormatter<ProcessStartInfoPackable?> f = ProcessStartInfoFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        /// <summary>
        /// 从 <see cref="MemoryPackReader"/> 对象反序列化为可空 <see cref="ProcessStartInfoPackable"/> 对象
        /// </summary>
        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref ProcessStartInfoPackable? value)
        {
            IMemoryPackFormatter<ProcessStartInfoPackable?> f = ProcessStartInfoFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

/// <summary>
/// <see cref="ProcessStartInfoPackable"/> 格式化程序
/// </summary>
public sealed class ProcessStartInfoPackableFormatterAttribute : MemoryPackCustomFormatterAttribute<ProcessStartInfoFormatter, ProcessStartInfoPackable>
{
    /// <summary>
    /// 获取 <see cref="ProcessStartInfoFormatter.Default"/> 实例
    /// </summary>
    public sealed override ProcessStartInfoFormatter GetFormatter() => ProcessStartInfoFormatter.Default;

    /// <summary>
    /// <see cref="ProcessStartInfoPackable"/> 格式化程序
    /// </summary>
    public sealed class Formatter : MemoryPackFormatter<ProcessStartInfoPackable>
    {
        /// <summary>
        /// 默认的 <see cref="Formatter"/> 实例
        /// </summary>
        public static readonly Formatter Default = new();

        /// <summary>
        /// 将 <see cref="ProcessStartInfoPackable"/> 对象序列化到 <see cref="MemoryPackWriter{TBufferWriter}"/> 对象
        /// </summary>
        /// <typeparam name="TBufferWriter"></typeparam>
        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref ProcessStartInfoPackable value)
        {
            IMemoryPackFormatter<ProcessStartInfoPackable> f = ProcessStartInfoFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        /// <summary>
        /// 从 <see cref="MemoryPackReader"/> 对象反序列化为 <see cref="ProcessStartInfoPackable"/> 对象
        /// </summary>
        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref ProcessStartInfoPackable value)
        {
            IMemoryPackFormatter<ProcessStartInfoPackable> f = ProcessStartInfoFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}