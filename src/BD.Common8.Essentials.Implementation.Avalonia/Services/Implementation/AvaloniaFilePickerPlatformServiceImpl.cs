namespace BD.Common8.Essentials.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

sealed class AvaloniaFilePickerPlatformServiceImpl :
    IFilePickerPlatformService,
    IFilePickerPlatformService.IServiceBase,
    IFilePickerPlatformService.IOpenFileDialogService,
    IFilePickerPlatformService.ISaveFileDialogService
{
    IFilePickerPlatformService.IOpenFileDialogService IFilePickerPlatformService.OpenFileDialogService => this;

    IFilePickerPlatformService.ISaveFileDialogService IFilePickerPlatformService.SaveFileDialogService => this;

    sealed class FilePickerFileTypeWrapper : IFilePickerFileType
    {
        public required IReadOnlyList<FilePickerFileType> Values { get; init; }

        public IEnumerable<string>? GetPlatformFileType(DevicePlatform2 platform)
        {
            foreach (var value in Values)
            {
                if (value.Patterns != null)
                    foreach (var item in value.Patterns)
                        yield return item;
                if (value.MimeTypes != null)
                    foreach (var item in value.MimeTypes)
                        yield return item;
                if (value.AppleUniformTypeIdentifiers != null)
                    foreach (var item in value.AppleUniformTypeIdentifiers)
                        yield return item;
            }
        }
    }

    IFilePickerFileType IPresetFilePickerPlatformService.Images { get; } = new FilePickerFileTypeWrapper
    {
        Values = new[]
        {
            new FilePickerFileType("All Images")
            {
                Patterns = new[] { "*.webp", "*.png", "*.jpg", "*.jpeg", "*.gif", "*.bmp" },
                AppleUniformTypeIdentifiers = new[] { "public.png", "public.jpeg", "public.jpeg-2000", "com.compuserve.gif", "com.microsoft.bmp", },
                MimeTypes = new[] { "image/webp", "image/png", "image/jpeg", "image/gif", "image/bmp", },
            },
        },
    };

    IFilePickerFileType IPresetFilePickerPlatformService.Png { get; } = new FilePickerFileTypeWrapper
    {
        Values = new[]
        {
            new FilePickerFileType("PNG image")
            {
                Patterns = new[] { "*.png", },
                AppleUniformTypeIdentifiers = new[] { "public.png", },
                MimeTypes = new[] { "image/png", },
            },
        },
    };

    IFilePickerFileType IPresetFilePickerPlatformService.Jpeg { get; } = new FilePickerFileTypeWrapper
    {
        Values = new[]
        {
            new FilePickerFileType("JPEG image")
            {
                Patterns = new[] { "*.jpg", "*.jpeg", },
                AppleUniformTypeIdentifiers = new[] { "public.jpeg", "public.jpeg-2000", },
                MimeTypes = new[] { "image/jpeg", },
            },
        },
    };

    IFilePickerFileType IPresetFilePickerPlatformService.Videos { get; } = new FilePickerFileTypeWrapper() { Values = [] };

    IFilePickerFileType IPresetFilePickerPlatformService.Pdf { get; } = new FilePickerFileTypeWrapper
    {
        Values = new[]
        {
            new FilePickerFileType("PDF document")
            {
                Patterns = new[] { "*.pdf", },
                AppleUniformTypeIdentifiers = new[] { "com.adobe.pdf", },
                MimeTypes = new[] { "application/pdf", },
            },
        },
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsAppleUniformTypeIdentifier(string s)
    {
        if (s == "jpeg" || s.StartsWith("public.") || s.StartsWith("com."))
        {
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string? GetAbsoluteFilePath(IStorageItem? storageItem)
    {
        const string trim_s =
#if WINDOWS
            "file:///";
#else
            "file://";
#endif
        var value = storageItem?.Path?.ToString()?.TrimStart(trim_s);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static FilePickerFileType Convert(string name, IEnumerable<string>? extensions)
    {
        // https://docs.avaloniaui.net/docs/next/concepts/services/storage-provider/file-picker-options
        var result = new FilePickerFileType(name);
        if (extensions != null)
        {
#if IOS || MACOS || MACCATALYST
            HashSet<string> appleUniformTypeIdentifiers = [];
#else
#if !WINDOWS
            HashSet<string> mimeTypes = [];
#endif
            HashSet<string> patterns = [];
#endif
            foreach (var x in extensions)
            {
                if (x.Contains('/'))
                {
#if !WINDOWS && !(IOS || MACOS || MACCATALYST)
                    mimeTypes.Add(x);
#endif
                    continue;
                }
                else if (IsAppleUniformTypeIdentifier(x))
                {
#if IOS || MACOS || MACCATALYST
                    appleUniformTypeIdentifiers.Add(x);
#endif
                    continue;
                }
                else if (x.StartsWith('.'))
                {
#if !(IOS || MACOS || MACCATALYST)
                    patterns.Add($"*{x}");
#endif
                    continue;
                }
                else if (x.StartsWith('*'))
                {
#if !(IOS || MACOS || MACCATALYST)
                    patterns.Add(x);
#endif
                    continue;
                }
                else
                {
#if !(IOS || MACOS || MACCATALYST)
                    patterns.Add($"*.{x}");
#endif
                    continue;
                }
            }
#if IOS || MACOS || MACCATALYST
            if (appleUniformTypeIdentifiers.Count != 0)
            {
                result.AppleUniformTypeIdentifiers = appleUniformTypeIdentifiers.ToArray();
            }
#else
#if !WINDOWS
            if (mimeTypes.Count != 0)
            {
                result.MimeTypes = mimeTypes.ToArray();
            }
#endif
            if (patterns.Count != 0)
            {
                result.Patterns = patterns.ToArray();
            }
#endif
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IReadOnlyList<FilePickerFileType>? Convert(IFilePickerFileType? fileTypes)
    {
        if (fileTypes is EssentialsFilePickerFileType ava)
        {
            var items = ava.Values;
            if (items != null)
            {
                var query = from m in items
                            select new FilePickerFileType(m.Name)
                            {
                                Patterns = m.Patterns,
                                AppleUniformTypeIdentifiers = m.AppleUniformTypeIdentifiers,
                                MimeTypes = m.MimeTypes,
                            };
                return query.ToArray();
            }
            return null;
        }
        else if (fileTypes is FilePickerFileTypeWrapper impl)
        {
            return impl.Values;
        }
        else
        {
            var extensions = fileTypes?.GetPlatformFileType(DeviceInfo2.Platform());
            if (extensions.Any_Nullable())
            {
                return new FilePickerFileType[]
                {
                    Convert(string.Empty, extensions),
                };
            }
        }
        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IEnumerable<IFileResult> Convert(IReadOnlyList<IStorageFile> fileResults)
    {
        foreach (var fileResult in fileResults)
        {
            var filePath = GetAbsoluteFilePath(fileResult);
            if (!string.IsNullOrEmpty(filePath))
                yield return new FileResult(filePath);
        }
    }

    async Task<IEnumerable<IFileResult>> IFilePickerPlatformService.IOpenFileDialogService.PlatformPickAsync(PickOptions? options, bool allowMultiple)
    {
        var topLevel = AvaApplication.Current.GetMainWindowOrActiveWindowOrMainView();
        var storageProvider = topLevel?.StorageProvider;
        if (storageProvider == null || !storageProvider.CanOpen)
            return Array.Empty<IFileResult>();

        FilePickerOpenOptions options_ = new()
        {
            AllowMultiple = allowMultiple,
        };
        if (options != default)
        {
            if (options.PickerTitle != default)
            {
                options_.Title = options.PickerTitle;
            }
            if (options.FileTypes != default)
            {
                var filters = Convert(options.FileTypes);
                if (filters != default)
                {
                    options_.FileTypeFilter = filters;
                }
            }
        }

        var fileResults = await storageProvider.OpenFilePickerAsync(options_);

        if (fileResults.Any_Nullable())
        {
            return Convert(fileResults);
        }
        return Array.Empty<FileResult>();
    }

    async Task<SaveFileResult?> IFilePickerPlatformService.ISaveFileDialogService.PlatformSaveAsync(PickOptions? options)
    {
        var topLevel = AvaApplication.Current.GetMainWindowOrActiveWindowOrMainView();
        var storageProvider = topLevel?.StorageProvider;
        if (storageProvider == null || !storageProvider.CanSave)
            return null;

        FilePickerSaveOptions options_ = new();

        if (options != default)
        {
            if (options.PickerTitle != default)
            {
                options_.Title = options.PickerTitle;
            }
            if (options.FileTypes != default)
            {
                var filters = Convert(options.FileTypes);
                if (filters != default)
                {
                    options_.FileTypeChoices = filters;
                }
            }
            if (options.InitialFileName != default)
            {
                options_.SuggestedFileName = options.InitialFileName;
            }
        }

        var fileResult = await storageProvider.SaveFilePickerAsync(options_);
        var filePath = GetAbsoluteFilePath(fileResult);
        return string.IsNullOrEmpty(filePath) ? null : new(filePath);
    }
}
