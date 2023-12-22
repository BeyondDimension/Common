using BD.Common8.SourceGenerator.Bcl.Test;

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable IDE1006 // 命名样式

// https://learn.microsoft.com/zh-cn/dotnet/communitytoolkit/mvvm/generators/observableproperty

//Console.WriteLine(typeof(TodoService).FullName);

var s1 = TodoService.Current;
var s2 = TodoService.Current;
Console.WriteLine(s2.GetHashCode());
if (s1 != s2)
    throw new ArgumentOutOfRangeException(nameof(s2));

var ss = await Task2.InParallel(
    Enumerable.Range(0, 15).Select(static _ => Task.Run(() =>
    {
        var result = TodoService.Current;
        Console.WriteLine(
            $"ManagedThreadId: {Environment.CurrentManagedThreadId}, s: {result.GetHashCode()}");
        return result;
    })));
foreach (var item in ss)
{
    if (s1 != item)
        throw new ArgumentOutOfRangeException(nameof(item));
}

var m = new TodoModel();
var d = m.D2;
var vm = new TodoViewModel(m);

var c = vm.C;

Console.WriteLine("Wait ReadLine Exit!");
Console.ReadLine();

namespace BD.Common8.SourceGenerator.Bcl.Test
{
    [SingletonPartitionGenerated]
    partial class TodoService
    {
        public bool B { get; set; }
    }

    //[MP2Obj(MP2SerializeLayout.Sequential)]
    public sealed partial record class TodoModel : IFixedSizeMemoryPackable
    {
        public Dictionary<TestEnum, Dictionary<TestEnum, Dictionary<TestEnum, string>>>? C { get; set; }

        public string D { get; set; } = "";

        //[ObservableProperty]
        public string? D2;

        public int B { get; set; } = DefaultB;

        public const int DefaultB = 5;

        public List<double>? DB { get; set; }

        public List<double> DB2 { get; set; } = [];

        public HashSet<string> HS { get; set; } = [];

        public HashSet<string?> HS2 { get; set; } = [];

        public HashSet<string?> HS3 { get; set; } = DefaultHS3;

        public static readonly HashSet<string?> DefaultHS3 = ["aaa"];

        public Dictionary<TestEnum, string>? TE { get; set; }

        public Dictionary<TestEnum, int> TE2 { get; set; } = [];

        public override string ToString() => $"C: {C}, D: {D}";

        [global::MemoryPack.Internal.Preserve]
        static int global::MemoryPack.IFixedSizeMemoryPackable.Size => 2;
    }

    public enum TestEnum
    {
    }

    [ViewModelWrapperGenerated(typeof(TodoModel),
        Properties = [
            nameof(TodoModel.C),
            nameof(TodoModel.D),
        ])]
    partial class TodoViewModel
    {
        /// <inheritdoc cref="TodoModel.D"/>
        [MinLength(2)]
        public string D { get => _D(); set => _D(value); }
    }

    [SettingsPropertyGenerated(typeof(TodoModel))]
    public static partial class TodoSettings
    {
    }
}