using BD.Common8.SourceGenerator.Bcl.Test;

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

//var a1 = new C1Model();
//a1.SetC1Model();

//var a2 = new C2Model();
//a2.SetC2Model();

Console.WriteLine("Wait ReadLine Exit!");
Console.ReadLine();

namespace BD.Common8.SourceGenerator.Bcl.Test
{
    [CopyPropertiesGenerated]
    [CopyPropertiesGenerated(destType: typeof(C2Model),
        IsExpression = true,
        IgnoreProperties = ["UsageSteamUserId"],
        MethodName = "P2SExpression",
        AppointProperties = "{\"UpdateTime\": \"UpdateTime ?? DateTimeOffset.Now\"}",
        MapProperties = "{\"UpdateTime\": \"UpdateTime2\"}")]
    [CopyPropertiesGenerated(destType: typeof(C2Model),
        AppointProperties = "{\"CreationTime\": \"CreationTime ?? DateTimeOffset.Now\"}",
        OnlyProperties = ["CreationTime"])]
    public class C1Model
    {
        public Guid Id { get; set; }

        public DateTimeOffset? UpdateTime { get; set; }

        public DateTimeOffset? CreationTime { get; set; }

        public Guid? OperatorUserId { get; set; }

        public Guid? CreateUserId { get; set; }

        public Guid? UsageSteamUserId { get; set; }
    }

    public partial class C2Model
    {
        public Guid Id { get; set; }

        public DateTimeOffset? UpdateTime2 { get; set; }

        public DateTimeOffset? CreationTime { get; set; }

        public Guid? OperatorUserId { get; set; }

        public Guid? CreateUserId { get; set; }

        public Guid? UsageSteamUserId { get; set; }
    }

    [SingletonPartitionGenerated]
    partial class TodoService
    {
        public bool B { get; set; }
    }

    public interface ITodoModel1
    {
        string A { get; }
    }

    //[MP2Obj(MP2SerializeLayout.Sequential)]
    public sealed partial record class TodoModel : IFixedSizeMemoryPackable, ITodoModel1
    {
        string ITodoModel1.A => string.Empty;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoViewModel"/> class.
    /// </summary>
    /// <param name="model"></param>
    [ViewModelWrapperGenerated(typeof(TodoModel),
        Constructor = false,
        Properties = [
            nameof(TodoModel.C),
            nameof(TodoModel.D),
        ])]
    partial class TodoViewModel(TodoModel model)
    {
        [MPIgnore, MP2Ignore]
        public TodoModel Model { get; } = model;

        /// <inheritdoc cref="TodoModel.D"/>
        [MinLength(2)]
        public string D { get => _D(); set => _D(value); }
    }

    [SettingsPropertyGenerated(typeof(TodoModel))]
    public static partial class TodoSettings
    {
    }

    /// <summary>
    /// <see cref="ReactiveObject"/> 的序列化忽略基类
    /// </summary>
    public abstract class ReactiveSerializationObject : ReactiveObject
    {
        /// <inheritdoc cref="ReactiveObject.Changing" />
        [XmlIgnore, IgnoreDataMember, SystemTextJsonIgnore, NewtonsoftJsonIgnore, MPIgnore, MP2Ignore]
        public new IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing => base.Changing;

        /// <inheritdoc cref="ReactiveObject.Changed" />
        [XmlIgnore, IgnoreDataMember, SystemTextJsonIgnore, NewtonsoftJsonIgnore, MPIgnore, MP2Ignore]
        public new IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed => base.Changed;

        /// <inheritdoc cref="ReactiveObject.ThrownExceptions" />
        [XmlIgnore, IgnoreDataMember, SystemTextJsonIgnore, NewtonsoftJsonIgnore, MPIgnore, MP2Ignore]
        public new IObservable<Exception> ThrownExceptions => base.ThrownExceptions;
    }
}