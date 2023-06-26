namespace BD.Common.Repositories.SourceGenerator;

static class Constants
{
    public const byte blank_space = 32;
    public const byte I = 73;

#if !PROJ_TRANSLATE

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ImmutableArray<T> GetInterfaces<T>()
    {
        var interfaceType = typeof(T);
        var handles = interfaceType.Assembly.GetTypes().
            Where(x => x.IsClass && !x.IsAbstract && interfaceType.IsAssignableFrom(x)).
            Select(x => (T)Activator.CreateInstance(x)).
            ToImmutableArray();
        return handles;
    }

    public static readonly Lazy<ImmutableArray<IPropertyHandle>> propertyHandles
        = new(GetInterfaces<IPropertyHandle>);

    //public static readonly Lazy<ImmutableArray<IAttributeHandle>> attributeHandles
    //    = new(GetInterfaces<IAttributeHandle>);

#endif

    /// <summary>
    /// 定义特性类型完整名称
    /// </summary>
    public static class TypeFullNames
    {
        public const string Comment = "Microsoft.EntityFrameworkCore.CommentAttribute";
        public const string MaxLength = "System.ComponentModel.DataAnnotations.MaxLengthAttribute";
        public const string MinLength = "System.ComponentModel.DataAnnotations.MinLengthAttribute";
        public const string Table = "System.ComponentModel.DataAnnotations.Schema.TableAttribute";
        public const string Description = "System.ComponentModel.DescriptionAttribute";
        public const string StringLength = "System.ComponentModel.DataAnnotations.StringLengthAttribute";
        public const string Url = "System.ComponentModel.DataAnnotations.UrlAttribute";
        public const string Range = "System.ComponentModel.DataAnnotations.RangeAttribute";
        public const string Required = "System.ComponentModel.DataAnnotations.RequiredAttribute";
        public const string Key = "System.ComponentModel.DataAnnotations.KeyAttribute";
        public const string DatabaseGenerated = "System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedAttribute";
        public const string Precision = "Microsoft.EntityFrameworkCore.PrecisionAttribute";
        public const string EmailAddress = "System.ComponentModel.DataAnnotations.EmailAddressAttribute";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetArgumentName(string argumentType)
    {
        var nameChars = argumentType.ToCharArray().AsSpan();
        if (argumentType.Length >= 2 &&
            nameChars[0] == 'I' &&
            nameChars[1] >= 'A' &&
            nameChars[1] <= 'Z')
        {
            nameChars = nameChars[1..];
        }
        nameChars[0] = char.ToLower(nameChars[0], CultureInfo.InvariantCulture);
        if (argumentType.EndsWith("Repository"))
        {
            nameChars = nameChars[..^"sitory".Length];
        }
        return new string(nameChars
#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER)
            .ToArray()
#endif
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task InBackground(Action action, bool longRunning = false)
    {
        TaskCreationOptions options = TaskCreationOptions.DenyChildAttach;

        if (longRunning)
        {
            options |= TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness;
        }

        await Task.Factory.StartNew(action, CancellationToken.None, options, TaskScheduler.Default).ConfigureAwait(false);
    }

    /// <summary>
    /// 后缀文件类型
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string GeFilExtensiont(string partialFileName) => partialFileName switch
    {
        "BackManageUIPage" => "tsx",
        "BackManageUIPageTypings" => "t.ts",
        "BackManageUIPageIndex" => "i.ts",
        "BackManageUIPageApi" => "a.ts",
        _ => "g.cs",
    };

    /// <summary>
    /// 获取文件路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string GetFilePath(string templateName, string moduleName, string tableName)
    {
        string pathTemplate = templateName switch
        {
            // c-sharp
            "Entity" => "{module}/{entity}.g.cs",
            "RepositoryImpl" => "{module}/{entity}Repository.g.cs",
            "Repository" => "{module}/I{entity}Repository.g.cs",
            "BackManageModel" => "{module}/{entity}DTOs.g.cs",
            "BackManageController" => "MicroServices/{module}/{entity}Controller.g.cs",

            // services
            "BackManageUIPageApi" => "{module}/{entity}/Generated/api.ts",
            "BackManageUIPageIndex" => "{module}/{entity}/Generated/index.ts",
            "BackManageUIPageTypings" => "{module}/{entity}/Generated/typing.ts",

            // pages
            "BackManageUIPage" => "{module}/Generated/{entity}Manage.tsx",
            _ => throw new NotImplementedException(),
        };

        string relativePath = pathTemplate
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace("{entity}", tableName)
            .Replace("{module}", moduleName);

        return relativePath;
    }
}