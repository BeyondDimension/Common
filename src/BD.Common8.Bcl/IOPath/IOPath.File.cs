namespace System;

public static partial class IOPath
{
    /// <summary>
    /// 如果指定的文件存在，则删除
    /// <para>可选择是否检查所在文件夹是否存在，不存在则创建文件夹</para>
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="notCreateDir"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FileIfExistsItDelete(string filePath, bool notCreateDir = false)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        else if (!notCreateDir)
        {
            var dirName = Path.GetDirectoryName(filePath);
            if (dirName != null)
            {
                DirCreateByNotExists(dirName);
            }
        }
    }

    /// <inheritdoc cref="FileIfExistsItDelete(string, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IfExistsItDelete(this FileInfo fileInfo, bool notCreateDir = false)
    {
        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }
        else if (!notCreateDir)
        {
            var dirName = Path.GetDirectoryName(fileInfo.FullName);
            if (dirName != null)
            {
                DirCreateByNotExists(dirName);
            }
        }
    }

    /// <summary>
    /// 尝试删除指定的文件
    /// <para>通常用于删除缓存</para>
    /// </summary>
    /// <param name="filePath"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FileTryDelete(string filePath)
    {
        try
        {
            File.Delete(filePath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc cref="File.Copy(string, string, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyFile(string sourceFileName, string destFileName, bool overwrite = true)
    {
        if (!File.Exists(sourceFileName))
            return;
        if (File.Exists(destFileName) && !overwrite)
            return;

        // Try copy the file normally - This will fail if in use
        var dirName = Path.GetDirectoryName(destFileName);
        if (!string.IsNullOrWhiteSpace(dirName)) // This could be a file in the working directory, instead of a file in a folder -> No need to create folder if exists.
            Directory.CreateDirectory(dirName);

        try
        {
            File.Copy(sourceFileName, destFileName, overwrite);
        }
        catch (Exception e)
        {
            // Try another method to copy.
            if (e.HResult == -2147024864) // File in use
            {
                if (File.Exists(destFileName))
                    File.Delete(destFileName);
                using var inputFile = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var outputFile = new FileStream(destFileName, FileMode.Create);
                var buffer = new byte[0x10000];
                int bytes;

                while ((bytes = inputFile.Read(buffer, 0, buffer.Length)) > 0)
                    outputFile.Write(buffer, 0, bytes);
            }
            else
            {
                throw;
            }
        }
    }

    /// <summary>
    /// Recursively copy files and directories
    /// </summary>
    /// <param name="inputFolder">Folder to copy files recursively from</param>
    /// <param name="outputFolder">Destination folder</param>
    /// <param name="overwrite">Whether to overwrite files or not</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyFilesRecursive(string? inputFolder, string outputFolder, bool overwrite = true)
    {
        _ = Directory.CreateDirectory(outputFolder);

        if (string.IsNullOrEmpty(inputFolder))
            return;

#pragma warning disable SA1114 // Parameter list should follow declaration
        outputFolder = outputFolder.EndsWith(
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            '\\'
#else
            "\\"
#endif
            ) ? outputFolder : outputFolder + '\\';
#pragma warning restore SA1114 // Parameter list should follow declaration

        //Now Create all of the directories
        foreach (var dirPath in Directory.GetDirectories(inputFolder, "*", SearchOption.AllDirectories))
            _ = Directory.CreateDirectory(dirPath.Replace(inputFolder, outputFolder));

        //Copy all the files & Replaces any files with the same name
        foreach (var newPath in Directory.GetFiles(inputFolder, "*.*", SearchOption.AllDirectories))
        {
            var dest = newPath.Replace(inputFolder, outputFolder);
            if (!overwrite && File.Exists(dest)) continue;

            File.Copy(newPath, dest, true);
        }
    }

    /// <summary>
    /// 允许文件共享的形式打开文件并读取文本
    /// </summary>
    /// <param name="path"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadAllText(string path, Encoding? encoding = null)
    {
        using var fileStream = OpenReadCore(path);
        using var streamReader = new StreamReader(fileStream, encoding ?? Encoding.UTF8);
        var result = streamReader.ReadToEnd();
        return result;
    }

    /// <summary>
    /// 允许文件共享的形式打开文件并异步读取文本
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default) => ReadAllTextAsync(path, null, cancellationToken);

    /// <summary>
    /// 允许文件共享的形式打开文件并异步读取文本
    /// </summary>
    /// <param name="path"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<string> ReadAllTextAsync(string path, Encoding? encoding, CancellationToken cancellationToken = default)
    {
        using var fileStream = OpenReadCore(path);
        using var streamReader = new StreamReader(fileStream, encoding ?? Encoding.UTF8);
#if NET7_0_OR_GREATER
        var result = await streamReader.ReadToEndAsync(cancellationToken);
#else
        cancellationToken.ThrowIfCancellationRequested();
        var result = await streamReader.ReadToEndAsync();
#endif
        return result ?? string.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MoveFile(string sourceFileName, string destFileName, bool overwrite)
    {
#if NETCOREAPP3_0_OR_GREATER
        File.Move(sourceFileName, destFileName, overwrite);
#else
        var moveOpt = overwrite
           ? MoveFileExFlag.MOVEFILE_REPLACE_EXISTING | MoveFileExFlag.MOVEFILE_WRITE_THROUGH
           : MoveFileExFlag.MOVEFILE_WRITE_THROUGH;

        if (!MoveFileEx(sourceFileName, destFileName, moveOpt))
        {
            int errorCode = Marshal.GetLastWin32Error();
            throw new Win32Exception(errorCode);
        }
#endif
    }

    [Flags]
    private enum MoveFileExFlag : uint
    {
        /// <summary>
        /// 如果存在名为 lpNewFileName 的文件，该函数会将其内容替换为 lpExistingFileName 文件的内容，前提是满足与访问控制列表（ACL）相关的安全要求。 有关详细信息，请参阅本主题的“备注”部分。
        /// 如果 lpNewFileName 为现有目录命名，则报告错误。
        /// </summary>
        MOVEFILE_REPLACE_EXISTING = 0x1,

        /// <summary>
        /// 如果要将文件移动到其他卷，该函数将使用 CopyFile 和 DeleteFile 函数模拟移动。
        /// 如果文件已成功复制到其他卷，并且无法删除原始文件，该函数会成功使源文件保持不变。
        /// 此值不能与 MOVEFILE_DELAY_UNTIL_REBOOT一起使用。
        /// </summary>
        MOVEFILE_COPY_ALLOWED = 0x2,

        /// <summary>
        /// 在重新启动操作系统之前，系统不会移动该文件。 系统在执行 AUTOCHK 后立即移动文件，但在创建任何分页文件之前。 因此，此参数使函数能够从以前的启动中删除分页文件。
        /// 仅当进程位于属于管理员组或 LocalSystem 帐户的用户的上下文中时，才能使用此值。
        /// 此值不能与 MOVEFILE_COPY_ALLOWED一起使用。
        /// </summary>
        MOVEFILE_DELAY_UNTIL_REBOOT = 0x4,

        /// <summary>
        /// 保留以供将来使用。
        /// </summary>
        MOVEFILE_CREATE_HARDLINK = 0x10,

        /// <summary>
        /// 如果源文件是链接源，但移动后无法跟踪该文件，则函数将失败。 如果目标是使用 FAT 文件系统格式化的卷，则可能会出现这种情况。
        /// </summary>
        MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x20,

        /// <summary>
        /// 在磁盘上实际移动文件之前，该函数不会返回。
        /// 设置此值可确保在函数返回之前将作为复制和删除操作执行的移动刷新到磁盘。 刷新发生在复制操作结束时。
        /// 如果设置了 MOVEFILE_DELAY_UNTIL_REBOOT，则此值不起作用。
        /// </summary>
        MOVEFILE_WRITE_THROUGH = 0x8
    }

    /// <summary>
    /// 移动文件/文件夹
    /// </summary>
    /// <param name="lpExistingFileName"></param>
    /// <param name="lpNewFileName"></param>
    /// <param name="dwFlags"></param>
    /// <see ref="https://learn.microsoft.com/zh-cn/windows/win32/api/winbase/nf-winbase-movefileexa"/>
    /// <returns></returns>
    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool MoveFileEx(
        string lpExistingFileName,
        string lpNewFileName,
        MoveFileExFlag dwFlags);
}