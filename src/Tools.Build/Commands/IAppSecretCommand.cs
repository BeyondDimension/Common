// <copyright file="IAppSecretCommand.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable SA1633 // File should have header
#pragma warning disable SA1629 // Documentation text should end with a period
#pragma warning disable SA1205 // Partial elements should declare access
#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable SA1310 // Field names should not contain underscore
#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter
#pragma warning disable SA1116 // Split parameters should start on line after declaration
#pragma warning disable SA1117 // Parameters should be on same line or separate lines
#pragma warning disable SA1513 // Closing brace should be followed by blank line
#pragma warning disable SA1123 // Do not place regions within elements
namespace Tools.Build.Commands;

/// <summary>
/// 部署 App 机密名命令
/// </summary>
partial interface IAppSecretCommand : ICommand
{
    /// <summary>
    /// 命令名
    /// </summary>
    const string CommandName = "secret";

    /// <inheritdoc cref="ICommand.GetCommand"/>
    static Command ICommand.GetCommand()
    {
        var path = new Option<string>("--path");
        var command = new Command(CommandName, "部署 App 机密名命令")
        {
            path,
        };
        command.SetHandler(Handler, path);
        return command;
    }

    /// <summary>
    /// 存放目标文件夹类型
    /// </summary>
    private enum DestDirType : byte
    {
        Parent,
        Root,
    }

    private static string GetDestFilePath(string fileName, DestDirType destDirType)
    {
        var dest_file = destDirType switch
        {
            DestDirType.Parent => Path.GetFullPath(Path.Combine(ROOT_ProjPath, "..", fileName)),
            DestDirType.Root => Path.Combine(Path.GetPathRoot(ROOT_ProjPath)!, fileName),
            _ => throw new ArgumentOutOfRangeException(nameof(destDirType), destDirType, null),
        };
        return dest_file;
    }

    private static void CreateEntryFromFile(ZipArchive archive, FileItem it)
    {
        var dest_file = GetDestFilePath(it.fileName, it.destType);
        if (it.isProtect)
        {
            var entry = archive.CreateEntry(it.fileName);
            var bytes = Unprotect(dest_file);
            using var stream = entry.Open();
            stream.Write(bytes);
        }
        else
        {
            archive.CreateEntryFromFile(dest_file, it.fileName);
        }
    }

    private static void CopyFile(string path, FileItem it)
    {
        var source_file = Path.Combine(path, it.fileName);
        var dest_file = GetDestFilePath(it.fileName, it.destType);
        if (source_file != null && File.Exists(source_file))
        {
            if (it.isProtect)
            {
                var bytes = File.ReadAllBytes(source_file);
                var bytes_e = Protect(dest_file, optionalEntropy_.Value);
                File.WriteAllBytes(dest_file, bytes_e);
            }
            else
            {
                File.Copy(source_file, dest_file, true);
            }
        }
    }

    private static byte[] Protect(string path, byte[]? optionalEntropy)
    {
        var userData = File.ReadAllBytes(path);
#pragma warning disable CA1416 // 验证平台兼容性
        var result = ProtectedData.Protect(userData, optionalEntropy, DataProtectionScope.LocalMachine);
#pragma warning restore CA1416 // 验证平台兼容性
        return result;
    }

    private static byte[] Unprotect(string path)
    {
        var encryptedData = File.ReadAllBytes(path);
#pragma warning disable CA1416 // 验证平台兼容性
        var result = ProtectedData.Unprotect(encryptedData, optionalEntropy_.Value, DataProtectionScope.LocalMachine);
#pragma warning restore CA1416 // 验证平台兼容性
        return result;
    }

#pragma warning disable IDE1006 // 命名样式
    /// <summary>
    /// 文件项
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="destType">存放目标</param>
    /// <param name="isProtect">是否需要加密</param>
    private readonly record struct FileItem(string fileName, DestDirType destType, bool isProtect = false)
#pragma warning restore IDE1006 // 命名样式
    {
    }

    private static IEnumerable<FileItem> GetFileItems(bool isCopyFile)
    {
        #region Android 机密

        yield return new("net.steampp.jks", DestDirType.Parent);
        yield return new("Xamarin.Android.KeyStore.props", DestDirType.Parent);

        #endregion

        #region iOS/macOS 机密

        yield return new("Apple.CodesignKey.props", DestDirType.Parent);

        #endregion

        #region 强签名证书

        yield return new("Mobius.snk", DestDirType.Parent);

        #endregion

        #region 短信机密

        yield return new("Constants.Sms.cs", DestDirType.Parent);

        #endregion

        if (OperatingSystem.IsWindows())
        {
            yield return new("MSStore_CodeSigning.cer", DestDirType.Root);
            yield return new("MSStore_CodeSigning.pfx", DestDirType.Root);
            yield return new("MSStore_CodeSigning.pfx.txt", DestDirType.Root, true);

            yield return new("BeyondDimension_CodeSigning.cer", DestDirType.Root);
            yield return new("BeyondDimension_CodeSigning.pfx", DestDirType.Root);
            yield return new("BeyondDimension_CodeSigning.pfx.txt", DestDirType.Root, true);
        }

        yield return new("IAppSecretCommand.cs", DestDirType.Parent);
    }

    static readonly Lazy<byte[]> optionalEntropy_ = new(() =>
    {
        var filePath = GetDestFilePath("optionalEntropy.txt", DestDirType.Root);
        if (File.Exists(filePath))
        {
            var result = File.ReadAllBytes(filePath);
            return result;
        }
        else
        {
            var result = Hashs.ByteArray.SHA512(Encoding.UTF8.GetBytes(Random2.GenerateRandomString(byte.MaxValue))).Concat(Guid.NewGuid().ToByteArray()).ToArray();
            File.WriteAllBytes(filePath, result);
            return result;
        }
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    private static void Handler(string path)
    {
        try
        {
            if (path.EndsWith(FileEx.ZIP))
            {
                // 打包机密值为 zip 文件
                using var archive = ZipFile.Open(path, ZipArchiveMode.Create);
                foreach (var it in GetFileItems(false))
                {
                    CreateEntryFromFile(archive, it);
                }
            }
            else if (Directory.Exists(path))
            {
                // 手动解压 zip 文件到文件夹然后读取机密
                try
                {
                    foreach (var it in GetFileItems(true))
                    {
                        CopyFile(path, it);
                    }
                }
                finally
                {
                    Directory.Delete(path, true);
                }
            }

            Console.WriteLine("OK");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
    }
}