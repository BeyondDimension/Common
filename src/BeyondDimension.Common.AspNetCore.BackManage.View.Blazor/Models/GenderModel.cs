// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

public class GenderModel : ITitle
{
    public Gender? Id { get; set; }

    public string Title { get; set; } = "";

    public static readonly GenderModel Null = new()
    {
        Id = null,
        Title = Strings.None,
    };

    public static readonly GenderModel Unkown = new()
    {
        Id = Gender.Unkown,
        Title = Strings.GenderUnkown,
    };

    public static readonly GenderModel Male = new()
    {
        Id = Gender.Male,
        Title = Strings.GenderMale,
    };

    public static readonly GenderModel Female = new()
    {
        Id = Gender.Female,
        Title = Strings.GenderFemale,
    };

    public static readonly GenderModel[] Items = new[] { Null, Male, Female, Unkown, };
}
