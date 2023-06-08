namespace BD.Common.Repositories.SourceGenerator;

static class Constants
{
    public const byte blank_space = 32;
    public const byte I = 73;

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
}
