using BD.Common8.SourceGenerator.Bcl.Test.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace BD.Common8.SourceGenerator.Bcl.Test.ViewModels;

/// <summary>
/// Initializes a new instance of the <see cref="TodoViewModel"/> class.
/// </summary>
/// <param name="model"></param>
[ViewModelWrapperGenerated(typeof(TodoModel),
    Constructor = false,
    Properties = [
            nameof(TodoModel.Doubles),
            nameof(TodoModel.Strings),
        ])]
partial class TodoViewModel(TodoModel model)
{
    [global::MessagePack.IgnoreMember, global::MemoryPack.MemoryPackIgnore]
    public TodoModel Model { get; } = model;

    /// <inheritdoc cref="TodoModel.Doubles"/>
    [MinLength(2)]
    public string? Doubles { get; set; }
}

[ViewModelWrapperGenerated(typeof(TodoModel),
    Constructor = true,
    Properties = [
            nameof(TodoModel.Doubles),
            nameof(TodoModel.Strings),
        ])]
partial class TodoViewModel2
{

}