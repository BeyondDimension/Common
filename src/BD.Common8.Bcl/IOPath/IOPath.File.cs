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
}