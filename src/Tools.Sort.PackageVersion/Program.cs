using Tools.Build.Commands;

switch (args.FirstOrDefault())
{
    case "copypkg": // 复制 release 发布的 pkg 包到 library-packs 目录
        {
            try
            {
                var pkgPath = Path.Combine(ROOT_ProjPath, "pkg");
                if (Directory.Exists(pkgPath))
                {
                    var files = Directory.GetFiles(pkgPath);
                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileName(file);
                        File.Move(file, Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                            "dotnet",
                            "library-packs",
                            fileName), true);
                    }
                }
            }
            finally
            {
                Console.WriteLine("OK");
            }
        }
        return;
    case "packref_new_migrate":
        {
            var dirPath = Path.Combine(ProjPath, "src", "Sdk", "PackageReference");
            foreach (var item in Directory.GetFiles(dirPath))
            {
                File.WriteAllBytes(item,
"""
<Project>
	<!-- 包引用 -->
	<ItemGroup Condition="$(MSBuildProjectName) != $(MSBuildThisFileName)">
		<PackageReference Include="$(MSBuildThisFileName)" />
		<PackageReference Include="BD.Common8.SourceGenerator.ResX" />
	</ItemGroup>
</Project>
"""u8.ToArray());
                var fileName = Path.GetFileName(item);
                var filePath2 = Path.Combine(ProjPath, "src", "Sdk", "buildTransitive", fileName);
                if (!File.Exists(filePath2))
                {
                    File.WriteAllBytes(filePath2,
"""
<Project>
	<Import Project="$(MSBuildThisFileDirectory)GlobalUsings.$(MSBuildThisFileName).props" />
</Project>
"""u8.ToArray());
                }
            }
        }
        return;
}

// NuGet 包清单排序

var filePath = Path.Combine(ProjPath, "src", "Directory.Packages.props");
var lines = await File.ReadAllLinesAsync(filePath);

List<PackageVersionItemGroup> pvigs = new();
PackageVersionItemGroup pvig = new();
var slnFileNames = IBuildCommand.GetSlnFileNames();
bool isForProjectLine = false;
for (int i = 0; i < lines.Length; i++)
{
    var line = lines[i].Trim();
    if (string.IsNullOrWhiteSpace(line))
        continue;

    if (string.Equals(line, "<Project>", StringComparison.OrdinalIgnoreCase))
    {
        isForProjectLine = true;
        continue;
    }

    if (!isForProjectLine)
        continue;

    try
    {
        if (line.StartsWith("<!--"))
        {
            line = line.TrimStart("<!--").TrimEnd("-->").Trim();
            pvig.Comments = line;
            if (slnFileNames.Contains(line))
            {
                pvig.Pairs = new Dictionary<string, string>();
            }
            continue;
        }
        else if (line.Equals("<ItemGroup>", StringComparison.OrdinalIgnoreCase))
        {
            if (pvig.LineNumber == -1)
            {
                pvig.LineNumber = i;
            }
            else
            {
                pvigs.Add(pvig);
                pvig = new();
            }
        }
        else if (line.Equals("</ItemGroup>", StringComparison.OrdinalIgnoreCase))
        {
            pvigs.Add(pvig);
            pvig = new();
        }
        var element = XElement.Parse(line);
        if (element.Name == "PackageVersion" ||
           element.Name == "PackageReference")
        {
            var include = element.Attribute("Include")!.Value;
            var version = element.Attribute("Version")!.Value;
            pvig.Pairs.Add(include, version);
            continue;
        }
    }
    catch
    {
    }
}
if (pvig.LineNumber != -1)
    pvigs.Add(pvig);

using MemoryStream stream = new();
stream.Write(
"""
<!-- NuGet 错误 NU1011 PackageVersion 项不能包含浮动版本。 -->
<Project>

"""u8);

foreach (var pvig_item in pvigs)
{
    if (!string.IsNullOrWhiteSpace(pvig_item.Comments))
    {
        stream.WriteFormat(
"""
	<!-- {0} -->

"""u8, pvig_item.Comments);
    }
    stream.Write(
"""
	<ItemGroup>

"""u8);
    foreach (var pair in pvig_item.Pairs)
    {
        stream.Write(
"""
		<PackageVersion Include="
"""u8);
        stream.Write(Encoding.UTF8.GetBytes(pair.Key));
        stream.Write(
"""
" Version="
"""u8);
        stream.Write(Encoding.UTF8.GetBytes(pair.Value));
        stream.Write(
"""
" />

"""u8);
    }
    stream.Write(
"""
	</ItemGroup>

"""u8);
}

stream.Write(
"""
</Project>

"""u8);

await File.WriteAllBytesAsync(filePath, stream.ToArray());

sealed record class PackageVersionItemGroup
{
    public IDictionary<string, string> Pairs { get; set; } = new SortedDictionary<string, string>();

    public string? Comments { get; set; }

    public int LineNumber { get; set; } = -1;
}