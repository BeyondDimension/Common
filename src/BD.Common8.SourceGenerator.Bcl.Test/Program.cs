#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable IDE1006 // 命名样式

// https://learn.microsoft.com/zh-cn/dotnet/communitytoolkit/mvvm/generators/observableproperty

using BD.Common8.SourceGenerator.Bcl.Test;

Console.WriteLine(typeof(TodoService).FullName);

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

    [MP2Obj(MP2SerializeLayout.Sequential)]
    public sealed record class TodoModel
    {
        public bool C { get; set; }

        public string D { get; set; } = "";

        //[ObservableProperty]
        public string? D2;

        public override string ToString() => $"C: {C}, D: {D}";
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
}