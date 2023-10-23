namespace BD.Common8.AssemblyLoad.Sample.Library;

#pragma warning disable SA1600 // Elements should be documented

static class Program
{
    static int Main(string[] args)
    {
        Console.WriteLine("in BD.Common8.AssemblyLoad.Sample.Library");
        Console.WriteLine("args: ", string.Join(" ", args));
        return DateTime.UtcNow.Year;
    }
}
