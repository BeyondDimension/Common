//using BD.Common8.SourceGenerator.Bcl.Models.Repositories;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;
//using System.Xml.Serialization;

//namespace BD.Common.Repositories.SourceGenerator.Templates;

///// <summary>
///// JsonEntityDesignMetadata 生成模板
///// </summary>
//sealed class JsonEntityDesignMetadataTemplate : TemplateBase<JsonEntityDesignMetadataTemplate, JsonEntityDesignMetadataTemplate.Metadata>
//{
//    public readonly record struct Metadata(
//        string Namespace,
//        string Summary,
//        string ClassName,
//        string? PrimaryKeyTypeName = null,
//        GenerateRepositoriesAttribute GenerateRepositoriesAttribute = null!) : ITemplateMetadata
//    {
//        /// <inheritdoc cref="GenerateRepositoriesAttribute.ApiControllerConstructorArguments"/>
//        public string[]? ConstructorArguments => GenerateRepositoriesAttribute.ApiControllerConstructorArguments;

//        /// <inheritdoc cref="GenerateRepositoriesAttribute.ApiRoutePrefix"/>
//        public string? ApiRoutePrefix => GenerateRepositoriesAttribute.ApiRoutePrefix;

//        /// <inheritdoc cref="GenerateRepositoriesAttribute.ApiRouteIgnoreRedundantEntityPrefix"/>
//        public bool ApiRouteIgnoreRedundantEntityPrefix => GenerateRepositoriesAttribute.ApiRouteIgnoreRedundantEntityPrefix;

//        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanAdd"/>
//        public bool BackManageCanAdd => GenerateRepositoriesAttribute.BackManageCanAdd;

//        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanDelete"/>
//        public bool BackManageCanDelete => GenerateRepositoriesAttribute.BackManageCanDelete;

//        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanEdit"/>
//        public bool BackManageCanEdit => GenerateRepositoriesAttribute.BackManageCanEdit;

//        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageEditModelReadOnly"/>
//        public bool BackManageEditModelReadOnly => GenerateRepositoriesAttribute.BackManageEditModelReadOnly;

//        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanTable"/>
//        public bool BackManageCanTable => GenerateRepositoriesAttribute.BackManageCanTable;
//    }

//    protected override void WriteCore(Stream stream, object?[] args, Metadata metadata, ImmutableArray<PropertyMetadata> fields)
//    {
//        EntityDesignMetadata m = new()
//        {
//            Attribute = metadata.GenerateRepositoriesAttribute,
//            Name = metadata.ClassName,
//            Properties = fields.ToDictionary(p => p.Name, p => new EntityDesignPropertyMetadata
//            {
//                Name = p.Name,
//                TypeName = p.PropertyType,
//                Attribute = p.BackManageField
//            })
//        };

//#pragma warning disable RS1035 // 不要使用禁用于分析器的 API
//        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)!, "JsonEntityDesignMetadataTemplate", metadata.ClassName + ".json");
//        var dirPath = Path.GetDirectoryName(filePath);
//        if (!Directory.Exists(dirPath))
//            Directory.CreateDirectory(dirPath);
//        using var stream2 = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
//#pragma warning restore RS1035 // 不要使用禁用于分析器的 API
//        var json = JsonConvert.SerializeObject(m, Formatting.Indented, new JsonSerializerSettings
//        {
//            NullValueHandling = NullValueHandling.Ignore,
//            Converters = [new StringEnumConverter()],
//        });
//        stream2.Write(Encoding.UTF8.GetBytes(json));
//        stream2.Flush();
//    }
//}
