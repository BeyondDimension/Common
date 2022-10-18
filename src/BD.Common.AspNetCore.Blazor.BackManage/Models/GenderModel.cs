// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

public class GenderModel : ITitle
{
    public Gender? Id { get; set; }

    public string Title { get; set; } = "";

    public static readonly GenderModel Null = new()
    {
        Id = null,
        Title = SharedStrings.None,
    };

    public static readonly GenderModel Unknown = new()
    {
        Id = Gender.Unknown,
        Title = SharedStrings.GenderUnknown,
    };

    public static readonly GenderModel Male = new()
    {
        Id = Gender.Male,
        Title = SharedStrings.GenderMale,
    };

    public static readonly GenderModel Female = new()
    {
        Id = Gender.Female,
        Title = SharedStrings.GenderFemale,
    };

    public static readonly GenderModel[] Items = new[] { Null, Male, Female, Unknown, };
}
