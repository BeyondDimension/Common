// ReSharper disable once CheckNamespace
namespace System;

partial class IOPath
{
    /// <summary>
    /// 如果指定的文件存在，则删除
    /// <para>可选择是否检查所在文件夹是否存在，不存在则创建文件夹</para>
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="notCreateDir"></param>
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
}