// https://dotnet.github.io/docfx/index.html
// https://dotnet.github.io/docfx/docs/template.html?tabs=modern
// https://blog.markvincze.com/build-and-publish-documentation-and-api-reference-with-docfx-for-net-core-projects/

Process? process = null;
try
{
    process = Process.Start(new ProcessStartInfo
    {
        FileName = "docfx",
        Arguments = "docfx.json --serve",
        WorkingDirectory = ProjPath,
        UseShellExecute = false,
    });
    process?.WaitForExit();
}
catch (Exception ex)
{
    if (ex.Message.Contains("An error occurred trying to start process"))
    {
        Console.WriteLine(
"""
输入以下命令安装 DocFX 工具
dotnet tool update -g docfx

""");
    }
    Console.WriteLine(ex);
    Console.ReadLine();
}
finally
{
    if (process != null)
    {
        process.Kill();
        process.Dispose();
    }
}