namespace System;

public static partial class IOPath
{
    /// <summary>
    /// 判断路径是否为文件夹，返回 <see cref="FileInfo"/> 或 <see cref="DirectoryInfo"/>，<see langword="true"/> 为文件夹，<see langword="false"/> 为文件，路径不存在则为 <see langword="null"/>
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileInfo"></param>
    /// <param name="directoryInfo"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool? IsDirectory(string path, [NotNullWhen(false)] out FileInfo? fileInfo, [NotNullWhen(true)] out DirectoryInfo? directoryInfo)
    {
        fileInfo = new(path);
        if (fileInfo.Exists) // 路径为文件
        {
            directoryInfo = null;
            return false;
        }
        fileInfo = null;
        directoryInfo = new(path);
        if (directoryInfo.Exists) // 路径为文件夹
        {
            return true;
        }
        directoryInfo = null;
        return null;
    }

    /// <summary>
    /// 如果指定的文件夹不存在，则创建文件夹
    /// </summary>
    /// <param name="dirPath"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DirCreateByNotExists(string dirPath)
    {
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
        return dirPath;
    }

    /// <inheritdoc cref="DirCreateByNotExists(string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DirectoryInfo CreateByNotExists(this DirectoryInfo dirInfo)
    {
        if (!dirInfo.Exists) dirInfo.Create();
        return dirInfo;
    }

    /// <summary>
    /// 尝试删除指定的文件夹，默认将删除文件夹下的所有文件、子目录
    /// <para>通常用于删除缓存</para>
    /// </summary>
    /// <param name="dirPath"></param>
    /// <param name="noRecursive"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool DirTryDelete(string dirPath, bool noRecursive = false)
    {
        try
        {
            Directory.Delete(dirPath, !noRecursive);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 将现有文件夹复制到新文件夹
    /// </summary>
    /// <param name="sourceDir"></param>
    /// <param name="destinationDir"></param>
    /// <param name="recursive"></param>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        if (!Directory.Exists(destinationDir))
            Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, true);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }

    /// <summary>
    /// 将现有文件夹复制到新文件夹(xcopy)
    /// <para>https://docs.microsoft.com/zh-cn/windows-server/administration/windows-commands/xcopy</para>
    /// </summary>
    /// <param name="sourceDirName"></param>
    /// <param name="destDirName"></param>
    /// <param name="timeoutMilliseconds"></param>
    [SupportedOSPlatform("Windows")]
    public static void XCopyDirectory(string sourceDirName, string destDirName, int timeoutMilliseconds = 60000)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
        };
        using var p = Process.Start(psi);
        p!.Start();
        p.StandardInput.WriteLine($"xcopy \"{sourceDirName}\" \"{destDirName}\" /y &exit");
        p.WaitForExit(timeoutMilliseconds);
        p.Kill();
    }

    /// <summary>
    /// 将文件或目录及其内容移动或复制到新位置
    /// </summary>
    /// <param name="sourceDirName"></param>
    /// <param name="destDirName"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MoveDirectory(string sourceDirName, string destDirName)
    {
        try
        {
            Directory.Move(sourceDirName, destDirName);
        }
        catch
        {
            try
            {
                CopyDirectory(sourceDirName, destDirName);
            }
            catch
            {
#if NET5_0_OR_GREATER
                if (OperatingSystem.IsWindows())
#else
                if (OperatingSystem2.IsWindows())
#endif
                {
                    XCopyDirectory(sourceDirName, destDirName);
                }
                else
                {
                    throw;
                }
            }
            try
            {
                Directory.Delete(sourceDirName, true);
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// 将文件或目录及其内容移动或复制到新位置(异步与带进度)
    /// </summary>
    /// <param name="sourceDirName"></param>
    /// <param name="destDirName"></param>
    /// <param name="progress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public static async Task MoveDirectoryAsync(string sourceDirName, string destDirName, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
    {
        const float maxProgress = 100f;
        try
        {
            Directory.Move(sourceDirName, destDirName);
        }
        catch
        {
            float totalSize = 0f;
            float currentSize = 0f;
            var streams = new List<FileStream>();
            try
            {
                var tasks = new List<Task>();
                CopyDirectoryTaskList(sourceDirName, destDirName);
                if (progress != null) StartObserver();
                await Task.WhenAll(tasks);
                currentSize = totalSize;

                async void StartObserver(int millisecondsDelay = 1500)
                {
                    do
                    {
                        try
                        {
                            await Task.Delay(millisecondsDelay, cancellationToken);
                            progress.Report(currentSize / totalSize);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                    } while (currentSize < totalSize);
                }

                void CopyDirectoryTaskList(string sourceDir, string destinationDir)
                {
                    // Get information about the source directory
                    var dir = new DirectoryInfo(sourceDir);

                    // Check if the source directory exists
                    if (!dir.Exists)
                        throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

                    // Cache directories before we start copying
                    DirectoryInfo[] dirs = dir.GetDirectories();

                    // Create the destination directory
                    if (!Directory.Exists(destinationDir))
                        Directory.CreateDirectory(destinationDir);

                    async Task CopyFileAsync(FileInfo file)
                    {
                        float fileSize = 0;
                        FileStream? sourceStream = null, destStream = null;
                        try
                        {
                            sourceStream = file.Open(FileMode.Open, FileAccess.Read, FileShareReadWriteDelete);
                            fileSize = sourceStream.Length;
                            streams.Add(sourceStream);

                            string targetFilePath = Path.Combine(destinationDir, file.Name);
                            destStream = new FileStream(targetFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShareReadWriteDelete);

                            await sourceStream.CopyToAsync(destStream, cancellationToken);
                            await destStream.FlushAsync(cancellationToken);
                            destStream.SetLength(destStream.Position);
                        }
                        finally
                        {
                            if (sourceStream != null)
                            {
                                sourceStream.Dispose();
                                streams.Remove(sourceStream);
                            }
                            if (destStream != null)
                            {
                                destStream.Dispose();
                                streams.Remove(destStream);
                            }
                            currentSize += fileSize;
                        }
                    }

                    // Get the files in the source directory and copy to the destination directory
                    foreach (FileInfo file in dir.GetFiles())
                    {
                        totalSize += file.Length;
                        var task = CopyFileAsync(file);
                        tasks.Add(task);
                    }

                    // If recursive and copying subdirectories, recursively call this method
                    foreach (DirectoryInfo subDir in dirs)
                    {
                        string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                        CopyDirectoryTaskList(subDir.FullName, newDestinationDir);
                    }
                }
            }
            finally
            {
                foreach (var stream in streams)
                {
                    stream.Dispose();
                }
            }
        }
        progress?.Report(maxProgress);
    }

    /// <summary>
    /// 递归删除
    /// </summary>
    /// <param name="baseDir"></param>
    /// <param name="keepFolders"></param>
    /// <param name="throwOnError"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool RecursiveDelete(string baseDir, bool keepFolders, bool throwOnError = false)
        => RecursiveDelete(new DirectoryInfo(baseDir), keepFolders, throwOnError);

    /// <summary>
    /// 递归删除
    /// </summary>
    /// <param name="baseDir"></param>
    /// <param name="keepFolders"></param>
    /// <param name="throwOnError"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool RecursiveDelete(DirectoryInfo baseDir, bool keepFolders, bool throwOnError = false)
    {
        if (!baseDir.Exists)
            return true;

        try
        {
            foreach (var dir in baseDir.EnumerateDirectories())
            {
                RecursiveDelete(dir, keepFolders);
            }
            var files = baseDir.GetFiles();
            foreach (var file in files)
            {
                if (!file.Exists) continue;
                file.IsReadOnly = false;
                try
                {
                    file.Delete();
                }
                catch (Exception e)
                {
                    if (throwOnError)
                    {
                        Log.Error(nameof(IOPath), e, "Recursive Delete could not delete file.");
                    }
                }
            }

            if (keepFolders) return true;
            try
            {
                baseDir.Delete();
            }
            catch (Exception e)
            {
                if (throwOnError)
                {
                    Log.Error(nameof(IOPath), e, "Recursive Delete could not delete folder.");
                }
            }
            return true;
        }
        catch (UnauthorizedAccessException e)
        {
            Log.Error(nameof(IOPath), e, "RecursiveDelete failed");
            return false;
        }
    }
}