using static BD.Common8.SourceGenerator.ResX.Constants;
using static BD.Common8.SourceGenerator.ResX.Helpers.ResXSatelliteAssemblyHelper;

namespace BD.Common8.SourceGenerator.ResX.Test.Helpers;

static partial class ResXHelper
{
    /// <summary>
    /// 移除附属资源文件中的 comment 元素
    /// </summary>
    /// <param name="dirPath"></param>
    public static IGrouping<string, string>[] RemoveResXCommentSatelliteAssemblies(string dirPath)
    {
        List<(string key, string fileNameWithoutEx)> list = [];
        var files = Directory.GetFiles(dirPath, "*.resx");
        foreach (var item in files)
        {
            var fileNameWithoutEx = Path.GetFileNameWithoutExtension(item);
            if (!TryGetCultureNameByResXSatelliteFilePathPath(item, out var cultureName))
            {
                list.Add((fileNameWithoutEx, fileNameWithoutEx));
                continue;
            }
            var fileNameWithoutExSplit = fileNameWithoutEx.Split('.');
            list.Add((string.Join('.', fileNameWithoutExSplit.Take(fileNameWithoutExSplit.Length - 1)), fileNameWithoutEx));
            var lines = File.ReadAllLines(item);
            using var fileStream = File.OpenWrite(item);
            using var writer = new StreamWriter(fileStream, Encoding.UTF8);
            foreach (var line in lines)
            {
                if (line.Contains("<comment>", StringComparison.OrdinalIgnoreCase))
                    continue;
                writer.WriteLine(line);
            }
            fileStream.SetLength(fileStream.Position);
            fileStream.Flush();
        }
        var result = list.GroupBy(static x => x.key, static x => x.fileNameWithoutEx).ToArray();
        return result;
    }

    static FileStream GetStream(string filePath)
    {
        var baseDirPath = Path.GetDirectoryName(filePath);
        ArgumentNullException.ThrowIfNull(baseDirPath);
        if (!Directory.Exists(baseDirPath))
            Directory.CreateDirectory(baseDirPath);
        var stream = new FileStream(filePath,
            FileMode.OpenOrCreate,
            FileAccess.Write,
            FileShare.ReadWrite | FileShare.Delete);
        return stream;
    }

    /// <summary>
    /// 写入 .props 文件
    /// </summary>
    /// <param name="dirPath"></param>
    /// <param name="values"></param>
    public static void WriteProps(string dirPath, IGrouping<string, string>[] values)
    {
        foreach (var item in values)
        {
            var propsFilePath = Path.Combine(dirPath, $"{item.Key}.props");
            using var stream = GetStream(propsFilePath);
            stream.Write(
"""
<Project>
	<ItemGroup>
"""u8);
            stream.WriteNewLine();
            var satellites = item.OrderBy(static x => x).ToArray();
            foreach (var satellite in satellites)
            {
                if (satellite == item.Key)
                {
                    string @namespace = satellite switch
                    {
                        "BD.Common8.Bcl" => "BD.Common8",
                        _ => satellite,
                    };
#pragma warning disable format
//                    stream.WriteFormat(
//"""
//                		<AdditionalFiles Include="\{0}.resx" Visible="false">
//                			<BD_Common8_Resx_IsPublic>false</BD_Common8_Resx_IsPublic>
//                			<BD_Common8_Resx_Namespace>{1}.Resources</BD_Common8_Resx_Namespace>
//                			<BD_Common8_Resx_CustomTypeName>SR</BD_Common8_Resx_CustomTypeName>
//                			<BD_Common8_Resx_CustomResourceBaseName>FxResources.{0}.SR</BD_Common8_Resx_CustomResourceBaseName>
//                		</AdditionalFiles>
//                """u8, satellite, @namespace);

                    // AdditionalText 在 macOS 上无效
//                    stream.WriteFormat(
//"""
//                		<AdditionalFiles Include="\{0}.resx" Visible="false">
//                			<!-- 使用 AdditionalFiles 引入主 resx 文件用于源生成器 -->
//                		</AdditionalFiles>
//                """u8, satellite, @namespace);
//                    stream.WriteNewLine();
#pragma warning restore format
                }
                stream.WriteFormat(
"""
		<EmbeddedResource Include="$(MSBuildThisFileDirectory){0}.resx">
			<Link>Resources\{1}.resx</Link>
			<LogicalName>FxResources.{0}.resources</LogicalName>
		</EmbeddedResource>
"""u8, satellite, satellite.Replace(item.Key, "Strings"));
                stream.WriteNewLine();
            }
            stream.Write(
"""
	</ItemGroup>
</Project>
"""u8);
            stream.SetLength(stream.Position);
            stream.Flush();

            WriteCSharpSR(dirPath, item.Key);

            static void WriteCSharpSR(string dirPath, string projName)
            {
                var resxFilePath = Path.Combine(dirPath, $"{projName}.resx");
                var dirPathCSharpSR = Path.Combine(ProjPath, "src", projName, "Resources");
                var filePathCSharpSR = Path.Combine(dirPathCSharpSR, "SR.ResXGeneratedCodeAttributes.cs");
                if (!Directory.Exists(dirPathCSharpSR)) Directory.CreateDirectory(dirPathCSharpSR);

                var @namespace = GetDefaultNamespaceByProjectName(projName);
                using var stream = GetStream(filePathCSharpSR);
                stream.Write(
"""
﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具 BD.Common8.SourceGenerator.ResX.Test.Helpers.ResXHelper 生成。
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配

"""u8);
                stream.WriteFormat(
"""
namespace {0};
"""u8, @namespace);
                stream.Write(
"""

#pragma warning restore IDE0079 // 请删除不必要的忽略


"""u8);
                var relativeFilePathSplit = Path.GetRelativePath(dirPathCSharpSR, resxFilePath);
                if (Path.DirectorySeparatorChar != '\\')
                    relativeFilePathSplit = relativeFilePathSplit.Replace(Path.DirectorySeparatorChar, '\\');
                stream.WriteFormat(
"""
[ResXGeneratedCode(@"{0}")]
"""u8, relativeFilePathSplit);
                stream.Write(
"""

partial class SR { }

"""u8);
                stream.SetLength(stream.Position);
                stream.Flush();
            }
        }
    }
}