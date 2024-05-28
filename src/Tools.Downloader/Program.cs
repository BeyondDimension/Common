using Downloader;
using ShellProgressBar;
using DownloadProgressChangedEventArgs = Downloader.DownloadProgressChangedEventArgs;

// https://github.com/bezzad/Downloader/tree/v3.1.0-beta/src/Samples/Downloader.Sample

namespace Tools.Downloader;

static partial class Program
{
    static List<DownloadItem> DownloadList = null!;
    static ProgressBar ConsoleProgress = null!;
    static ConcurrentDictionary<string, ChildProgressBar?> ChildConsoleProgresses = null!;
    static ProgressBarOptions ChildOption = null!;
    static ProgressBarOptions ProcessBarOption = null!;
    static IDownloadService CurrentDownloadService = null!;
    static DownloadConfiguration CurrentDownloadConfiguration = null!;
    static CancellationTokenSource CancelAllTokenSource = null!;

    static async Task Main()
    {
        try
        {
            Initial();
            Task2.InBackground(KeyboardHandler, true);
            await DownloadAll(DownloadList, CancelAllTokenSource.Token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            Debugger.Break();
        }
        finally
        {
            Console.WriteLine("END");
            Console.ReadLine();
        }
    }

    static void Initial()
    {
        CancelAllTokenSource = new();
        ChildConsoleProgresses = new();
        DownloadList = GetDownloadItems();

        ProcessBarOption = new()
        {
            ForegroundColor = ConsoleColor.Green,
            ForegroundColorDone = ConsoleColor.DarkGreen,
            BackgroundColor = ConsoleColor.DarkGray,
            BackgroundCharacter = '\u2593',
            EnableTaskBarProgress = true,
            ProgressBarOnBottom = false,
            ProgressCharacter = '#',
        };
        ChildOption = new()
        {
            ForegroundColor = ConsoleColor.Yellow,
            BackgroundColor = ConsoleColor.DarkGray,
            ProgressCharacter = '-',
            ProgressBarOnBottom = true,
        };
    }

    static void KeyboardHandler()
    {
        ConsoleKeyInfo cki;
        Console.CancelKeyPress += CancelAll;

        while (true)
        {
            cki = Console.ReadKey(true);
            if (CurrentDownloadConfiguration != null)
            {
                switch (cki.Key)
                {
                    case ConsoleKey.P:
                        CurrentDownloadService?.Pause();
                        Console.Beep();
                        break;
                    case ConsoleKey.R:
                        CurrentDownloadService?.Resume();
                        break;
                    case ConsoleKey.Escape:
                        CurrentDownloadService?.CancelAsync();
                        break;
                        //case ConsoleKey.UpArrow:
                        //    CurrentDownloadConfiguration.MaximumBytesPerSecond *= 2;
                        //    break;
                        //case ConsoleKey.DownArrow:
                        //    CurrentDownloadConfiguration.MaximumBytesPerSecond /= 2;
                        //    break;
                }
            }
        }
    }

    static void CancelAll(object? sender, ConsoleCancelEventArgs e)
    {
        CancelAllTokenSource.Cancel();
        CurrentDownloadService?.CancelAsync();
    }

    static List<DownloadItem> GetDownloadItems()
    {
        var cacheDir = Path.Combine(AppContext.BaseDirectory, "Cache");
        Console.WriteLine($"Download cache directory: {cacheDir}");
        return
            [
                //new DownloadItem
                //{
                //    FolderPath = cacheDir,
                //    Url = DotNetRuntimeDownloadHelper.GetDownloadUrl(DotNetRuntimeDownloadHelper.DownloadType_DotNet, "win-x64"),
                //},
                //new DownloadItem
                //{
                //    FolderPath = cacheDir,
                //    Url = DotNetRuntimeDownloadHelper.GetDownloadUrl(DotNetRuntimeDownloadHelper.DownloadType_AspNetCore, "win-x64"),
                //},
                new DownloadItem
                {
                    FolderPath = cacheDir,
                    Url = "https://download.visualstudio.microsoft.com/download/pr/77650902-a341-4f4c-934f-db7056cbfa78/176d961f8bbc798596f8d498ede4cc73/dotnet-runtime-8.0.5-win-x64.zip",
                },
            ];
    }

    static async Task DownloadAll(IEnumerable<DownloadItem> downloadList, CancellationToken cancelToken)
    {
        foreach (DownloadItem downloadItem in downloadList)
        {
            if (cancelToken.IsCancellationRequested)
                return;

            // begin download from url
            await DownloadFile(downloadItem).ConfigureAwait(false);
        }
    }

    static async Task<IDownloadService> DownloadFile(DownloadItem downloadItem)
    {
        CurrentDownloadConfiguration = GetDownloadConfiguration();
        CurrentDownloadService = CreateDownloadService(CurrentDownloadConfiguration);
        if (string.IsNullOrWhiteSpace(downloadItem.FileName))
        {
            await CurrentDownloadService.DownloadFileTaskAsync(downloadItem.Url, new DirectoryInfo(downloadItem.FolderPath!)).ConfigureAwait(false);
        }
        else
        {
            await CurrentDownloadService.DownloadFileTaskAsync(downloadItem.Url, downloadItem.FileName).ConfigureAwait(false);
        }

        return CurrentDownloadService;
    }

    static void WriteKeyboardGuidLines()
    {
        Console.WriteLine("按 Esc 键停止当前文件下载");
        Console.WriteLine("按 P 暂停下载，按 R 恢复下载");
        //Console.WriteLine("按向上箭头将下载速度提高 2 倍");
        //Console.WriteLine("按向下箭头将下载速度降低 2 倍");
        Console.WriteLine();
    }

    static DownloadService CreateDownloadService(DownloadConfiguration config)
    {
        var downloadService = new DownloadService(config);

        // Provide `FileName` and `TotalBytesToReceive` at the start of each downloads
        downloadService.DownloadStarted += OnDownloadStarted;

        // Provide any information about chunker downloads,
        // like progress percentage per chunk, speed,
        // total received bytes and received bytes array to live streaming.
        downloadService.ChunkDownloadProgressChanged += OnChunkDownloadProgressChanged;

        // Provide any information about download progress,
        // like progress percentage of sum of chunks, total speed,
        // average speed, total received bytes and received bytes array
        // to live streaming.
        downloadService.DownloadProgressChanged += OnDownloadProgressChanged;

        // Download completed event that can include occurred errors or
        // cancelled or download completed successfully.
        downloadService.DownloadFileCompleted += OnDownloadFileCompleted;

        return downloadService;
    }

    static void OnDownloadStarted(object? sender, DownloadStartedEventArgs e)
    {
        WriteKeyboardGuidLines();
        ConsoleProgress = new ProgressBar(10000, $"Downloading {Path.GetFileName(e.FileName)}   ", ProcessBarOption);
    }

    static void OnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
    {
        if (ConsoleProgress == null)
            return;

        ConsoleProgress.Tick(10000);

        if (e.Cancelled)
        {
            ConsoleProgress.Message += " CANCELED";
        }
        else if (e.Error != null)
        {
            Console.Error.WriteLine(e.Error);
            Debugger.Break();
        }
        else
        {
            ConsoleProgress.Message += " DONE";
            Console.Title = $"100% max: {max_speed.CalcMemoryMensurableUnit()}/s (avg: {avg_speed.CalcMemoryMensurableUnit()}/s)";
        }

        foreach (var child in ChildConsoleProgresses.Values)
            child?.Dispose();

        ChildConsoleProgresses.Clear();
        ConsoleProgress?.Dispose();
    }

    static void OnChunkDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
    {
        var progress = ChildConsoleProgresses.GetOrAdd(e.ProgressId,
            id => ConsoleProgress?.Spawn(10000, $"chunk {id}", ChildOption));
        progress?.Tick((int)(e.ProgressPercentage * 100));
        var activeChunksCount = e.ActiveChunks; // Running chunks count
    }

    static void OnDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
    {
        ConsoleProgress.Tick((int)(e.ProgressPercentage * 100));
        if (sender is DownloadService ds)
            e.UpdateTitleInfo(ds.IsPaused);
    }

    static string CalcMemoryMensurableUnit(this long bytes) => IOPath.GetDisplayFileSizeString(bytes);

    static string CalcMemoryMensurableUnit(this double bytes) => IOPath.GetDisplayFileSizeString(bytes);

    static double max_speed;
    static double avg_speed;

    static void UpdateTitleInfo(this DownloadProgressChangedEventArgs e, bool isPaused)
    {
        int estimateTime = (int)Math.Ceiling((e.TotalBytesToReceive - e.ReceivedBytesSize) / e.AverageBytesPerSecondSpeed);
        string timeLeftUnit = "seconds";

        if (estimateTime >= 60) // isMinutes
        {
            timeLeftUnit = "minutes";
            estimateTime /= 60;
        }
        else if (estimateTime < 0)
        {
            estimateTime = 0;
        }

        if (e.BytesPerSecondSpeed > max_speed)
        {
            max_speed = e.BytesPerSecondSpeed;
        }

        avg_speed = e.AverageBytesPerSecondSpeed;

        string avgSpeed = e.AverageBytesPerSecondSpeed.CalcMemoryMensurableUnit();
        string speed = e.BytesPerSecondSpeed.CalcMemoryMensurableUnit();
        string bytesReceived = e.ReceivedBytesSize.CalcMemoryMensurableUnit();
        string totalBytesToReceive = e.TotalBytesToReceive.CalcMemoryMensurableUnit();
        string progressPercentage = $"{e.ProgressPercentage:F3}".Replace("/", ".");
        string usedMemory = GC.GetTotalMemory(false).CalcMemoryMensurableUnit();

        Console.Title = $"{estimateTime} {timeLeftUnit} left   -  " +
                        $"{speed}/s (avg: {avgSpeed}/s)  -  " +
                        $"{progressPercentage}%  -  " +
                        $"[{bytesReceived} of {totalBytesToReceive}]   " +
                        $"Active Chunks: {e.ActiveChunks}   -   " +
                        $"[{usedMemory} memory]   " +
                        (isPaused ? " - Paused" : "");
    }
}

static partial class Program
{
    static DownloadConfiguration GetDownloadConfiguration()
    {
        var headers = new WebHeaderCollection();
        headers.Add("Accept-Encoding", "gzip, deflate, br");
        headers.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6");
        return new()
        {
            BufferBlockSize = 10240,    // usually, hosts support max to 8000 bytes, default values is 8000
            ChunkCount = 8,             // file parts to download, default value is 1
            //MaximumBytesPerSecond = 1024 * 1024 * 10,  // download speed limited to 10MB/s, default values is zero or unlimited
            MaxTryAgainOnFailover = 5,  // the maximum number of times to fail
            MaximumMemoryBufferBytes = 1024 * 1024 * 200, // release memory buffer after each 200MB
            ParallelDownload = true,    // download parts of file as parallel or not. Default value is false
            ParallelCount = 8,          // number of parallel downloads. The default value is the same as the chunk count
            Timeout = 3000,             // timeout (millisecond) per stream block reader, default value is 1000
            RangeDownload = false,      // set true if you want to download just a specific range of bytes of a large file
            RangeLow = 0,               // floor offset of download range of a large file
            RangeHigh = 0,              // ceiling offset of download range of a large file
            ClearPackageOnCompletionWithFailure = true, // Clear package and downloaded data when download completed with failure, default value is false
            MinimumSizeOfChunking = 1024, // minimum size of chunking to download a file in multiple parts, default value is 512
            ReserveStorageSpaceBeforeStartingDownload = false, // Before starting the download, reserve the storage space of the file as file size, default value is false
            RequestConfiguration =
            {
                // config and customize request headers
                Accept = "*/*",
                //CookieContainer = cookies,
                Headers = headers,     // { your custom headers }
                KeepAlive = true,                        // default value is false
                ProtocolVersion = HttpVersion.Version11, // default value is HTTP 1.1
                UseDefaultCredentials = false,
                // your custom user agent or your_app_name/app_version.
                UserAgent = UserAgentConstants.Win10ChromeLatest,
                // Proxy = new WebProxy(new Uri($"socks5://127.0.0.1:9050"))
                // Proxy = new WebProxy() {
                //    Address = new Uri("http://YourProxyServer/proxy.pac"),
                //    UseDefaultCredentials = false,
                //    Credentials = System.Net.CredentialCache.DefaultNetworkCredentials,
                //    BypassProxyOnLocal = true
                // }
            },
        };
    }
}

sealed record class DownloadItem
{
    public string? _folderPath;

    public string? FolderPath { get => _folderPath ?? Path.GetDirectoryName(FileName); init => _folderPath = value; }

    public string? FileName { get; init; }

    public required string Url { get; init; }
}