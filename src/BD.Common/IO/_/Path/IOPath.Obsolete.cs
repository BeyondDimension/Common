// ReSharper disable once CheckNamespace
namespace System;

partial class IOPath
{
    /// <summary>
    /// 获取资源文件路径
    /// </summary>
    /// <param name="resData">资源数据</param>
    /// <param name="resName">资源名称</param>
    /// <param name="resVer">资源文件版本</param>
    /// <param name="fileEx">资源文件扩展名</param>
    /// <returns></returns>
    [Obsolete]
    public static string GetFileResourcePath(byte[] resData, string resName, int resVer, string fileEx)
    {
        var dirPath = Path.Combine(AppDataDirectory, resName);
        var filePath = Path.Combine(dirPath, $"{resName}@{resVer}{fileEx}");
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
            WriteFile();
        }
        else
        {
            if (!File.Exists(filePath))
            {
                var oldFiles = Directory.GetFiles(dirPath);
                if (oldFiles != null)
                {
                    foreach (var oldFile in oldFiles)
                    {
                        FileTryDelete(oldFile);
                    }
                }
                WriteFile();
            }
        }
        void WriteFile() => File.WriteAllBytes(filePath, resData);
        return filePath;
    }

}