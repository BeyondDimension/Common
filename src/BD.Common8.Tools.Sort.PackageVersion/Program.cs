// NuGet 包清单排序

var filePath = Path.Combine(ProjPath, "src", "Directory.Packages.props");
var lines = await File.ReadAllLinesAsync(filePath);
SortedDictionary<string, string> dict = [];
List<string> startLines = [], endLines = [];
foreach (var line in lines)
{
    try
    {
        var element = XElement.Parse(line);
        if (element.Name == "PackageVersion")
        {
            var include = element.Attribute("Include")!.Value;
            var version = element.Attribute("Version")!.Value;
            dict.Add(include, version);
            continue;
        }
    }
    catch
    {
    }

    (dict.Count > 0 ? endLines : startLines).Add(line);
}
static void WriteLines(Stream stream, IEnumerable<string> lines)
{
    foreach (var line in lines)
    {
        stream.Write(Encoding.UTF8.GetBytes(line));
        stream.Write("\r\n"u8);
    }
}
using MemoryStream stream = new();
WriteLines(stream, startLines);
foreach (var pair in dict)
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
    stream.Write("\r\n"u8);
}
WriteLines(stream, endLines);
await File.WriteAllBytesAsync(filePath, stream.ToArray());