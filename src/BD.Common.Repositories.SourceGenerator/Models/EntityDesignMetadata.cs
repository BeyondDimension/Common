namespace BD.Common8.SourceGenerator.Bcl.Models.Repositories;

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

    public BackManageFieldAttribute? Attribute { get; set; }
}
