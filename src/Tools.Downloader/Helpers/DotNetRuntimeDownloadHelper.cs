namespace Tools.Downloader.Helpers;

static class DotNetRuntimeDownloadHelper
{
    public const string DownloadType_DotNet = "Microsoft.NETCore.App.Runtime";
    public const string DownloadType_AspNetCore = "Microsoft.AspNetCore.App.Runtime";

    public static string GetDownloadUrl(string downloadType, string rid, string? version = null)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            version = Environment.Version.ToString();
        }

        var url = $"https://www.nuget.org/api/v2/package/{downloadType}.{rid}/{version}";
        return url;
    }
}
