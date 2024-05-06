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

    public bool Required { get; set; }

    public int MaxLength { get; set; }

    public int Precision { get; set; }

    public int PrecisionScale { get; set; }

    public BackManageFieldAttribute? Attribute { get; set; }
}
