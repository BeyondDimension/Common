namespace AssemblyLoad.Sample.Library;

static class Program
{
    static int Main(string[] args)
    {
        Console.WriteLine("in AssemblyLoad.Sample.Library");
        Console.WriteLine("args: ", string.Join(" ", args));
        return DateTime.UtcNow.Year;
    }
}
