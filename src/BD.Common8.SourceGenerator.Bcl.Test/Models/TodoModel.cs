using BD.Common8.SourceGenerator.Bcl.Test.Enums;
using MemoryPack;

namespace BD.Common8.SourceGenerator.Bcl.Test.Models;

public sealed partial record class TodoModel : IFixedSizeMemoryPackable, ITodoModel
{
    string ITodoModel.Name => string.Empty;

    public Dictionary<TodoEnum, Dictionary<TodoEnum, Dictionary<TodoEnum, string>>>? Dict { get; set; }

    public string Name { get; set; } = "";

    public string? NameF;

    public int Length { get; set; } = DefaultLength;

    public const int DefaultLength = 5;

    public List<double>? Doubles { get; set; }

    public List<double> Doubles2 { get; set; } = [];

    public HashSet<string> Strings { get; set; } = [];

    public HashSet<string?> Strings2 { get; set; } = [];

    public HashSet<string?> Strings3 { get; set; } = DefaultStrings3;

    public static readonly HashSet<string?> DefaultStrings3 = ["aaa"];

    public Dictionary<TodoEnum, string>? EnumStringDict { get; set; }

    public Dictionary<TodoEnum, int> EnumInt32Dict { get; set; } = [];

    public Todo1Model? Todo1 { get; set; }

    public Todo2Model? Todo2 { get; set; }

    public List<Todo2Model?>? Todo2s { get; set; }

    public Dictionary<Todo1Model, Todo2Model?>? TodoDoct { get; set; }

    public override string ToString() => $"Name: {Name}, Length: {Length}";

    [global::MemoryPack.Internal.Preserve]
    static int global::MemoryPack.IFixedSizeMemoryPackable.Size => 2;
}