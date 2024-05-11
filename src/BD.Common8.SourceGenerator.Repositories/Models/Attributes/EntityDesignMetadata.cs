namespace BD.Common8.SourceGenerator.Repositories.Models.Attributes;

public sealed record class EntityDesignMetadata
{
    public string? Name { get; set; }

    public Dictionary<string, EntityDesignPropertyMetadata>? Properties { get; set; }

    public GenerateRepositoriesAttribute? Attribute { get; set; }
}

public sealed record class EntityDesignPropertyMetadata
{
    public string? Name { get; set; }

    public string? TypeName { get; set; }

    public string? DefaultValue { get; set; }

    public string? Comment { get; set; }

    public int? MaxLength { get; set; }

    public int? MinLength { get; set; }

    public string? Table { get; set; }

    public string? Description { get; set; }

    public int? StringLength { get; set; }

    public bool? Url { get; set; }

    public string? Range { get; set; }

    public bool? Required { get; set; }

    public string? Precision { get; set; }

    public bool? Key { get; set; }

    public string? DatabaseGenerated { get; set; }

    public bool? EmailAddress { get; set; }

    public BackManageFieldAttribute? Attribute { get; set; }
}
