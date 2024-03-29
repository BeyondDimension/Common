using System.Text;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var dirPath = @"\Common\src\Shared";
            var files = Directory.GetFiles(dirPath, "GlobalUsings.*.cs");
            await Parallel.ForEachAsync(files, (file, _) =>
             {
                 if (Path.GetFileName(file) == "GlobalUsings.Bcl.cs")
                     return default;

                 var content = File.ReadAllLines(file);
                 var anySharp = content.Where(x => x.Contains('#') && !x.Contains("#pragma") && !x.Contains("C#")).Any();
                 if (anySharp)
                 {
                     return default;
                 }
                 StringBuilder s = new(
 """
<Project>
	<ItemGroup>

""");
                 var hasValue = false;
                 foreach (var item in content)
                 {
                     if (item.StartsWith("global using"))
                     {
                         hasValue = true;
                         var value = item.Replace("global using", string.Empty).Trim();
                         if (value.Contains('='))
                         {
                             var values = value.Split('=', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

                             s.AppendFormat(
 """
		<Using Alias="{1}" Include="{0}" />

""", values[1], values[0]);
                         }
                         else
                         {
                             s.AppendFormat(
     """
		<Using Include="{0}" />

""", value);
                         }
                     }
                 }
                 s.Append(
 """
	</ItemGroup>
</Project>
""");
                 if (hasValue)
                 {
                     File.WriteAllText(file.Replace(".cs", ".props"), s.ToString());
                     File.Delete(file);
                 }
                 return default;
             });
            Console.WriteLine("OK");
        }
    }
}
