namespace BD.Common8.SourceGenerator.Repositories.Models.Attributes;

public sealed record class EntityDesignMetadata
{
    public string[] _ { get; set; } = [
    "此代码由工具 SPP.Glossaries 生成。",
    "对此文件的更改可能会导致不正确的行为，并且如果",
    "重新生成代码，这些更改将会丢失。",
    "更改 ndml2 设计文件，然后重新生成此代码。"];

    public string? Name { get; set; }

    public Dictionary<string, EntityDesignPropertyMetadata>? Properties { get; set; }

    public GenerateRepositoriesAttribute? Attribute { get; set; }

    public string? Summary { get; set; }
}

public sealed record class EntityDesignPropertyMetadata
{
    public string? Name { get; set; }

    public string? TypeName { get; set; }

    public string? DefaultValue { get; set; }

    public string? Comment { get; set; }

    public string? Column { get; set; }

    public string? MaxLength { get; set; }

    public string? MinLength { get; set; }

    public string? Description { get; set; }

    public string? StringLength { get; set; }

    public bool? Url { get; set; }

    public string? Range { get; set; }

    public bool? Required { get; set; }

    public string? Precision { get; set; }

    public bool? Key { get; set; }

    public string? DatabaseGenerated { get; set; }

    public bool? EmailAddress { get; set; }

    public string? Summary { get; set; }

    public string[]? PreprocessorDirective { get; set; }

    public string? Modifier { get; set; }

    public bool IsValueType { get; set; }

    public BackManageFieldAttribute? Attribute { get; set; }
}
