global using static BD.Common8.SourceGenerator.Repositories.Helpers.GlobalHelper;

namespace BD.Common8.SourceGenerator.Repositories.Helpers;

static class GlobalHelper
{
    public const byte blank_space = 32;
    public const byte I = 73;

    static ImmutableArray<T> GetInterfaces<T>()
    {
        var interfaceType = typeof(T);
        var handles = interfaceType.Assembly.GetTypes().
            Where(x => x.IsClass && !x.IsAbstract && interfaceType.IsAssignableFrom(x)).
            Select(x => (T)Activator.CreateInstance(x)).
            ToImmutableArray();
        return handles;
    }

    public static readonly Lazy<ImmutableArray<IPropertyHandle>> PropertyHandles
        = new(GetInterfaces<IPropertyHandle>);

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
    /// 获取文件路径
    /// </summary>
    /// <param name="templateName"></param>
    /// <param name="moduleName"></param>
    /// <param name="tableName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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
            "BackManageUIPageTypings" => "{module}/{entity}/Generated/typings.d.ts",

            // pages
            "BackManageUIPage" => "{module}/Generated/{entity}Manage.tsx",
            _ => throw new ArgumentOutOfRangeException(nameof(templateName)),
        };

        string relativePath = pathTemplate
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace("{entity}", tableName)
            .Replace("{module}", moduleName);

        return relativePath;
    }

    public static string ParseAgilePath(string path)
    {
        var segments = path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).ToList();
        for (int i = segments.Count - 1; i >= 0; i--)
        {
            if (segments[i] == "^..")
            {
                // 跳过一下个目录
                segments.RemoveAt(i);
                segments.RemoveAt(i);
            }
        }
        return Path.Combine([.. segments]);
    }
}
