namespace BD.Common.Repositories.SourceGenerator.Handlers.Attributes;

sealed class GeneralAttributeHandle : IAttributeHandle
{
    public static IAttributeHandle Instance { get; } = new GeneralAttributeHandle();

    static readonly Lazy<ImmutableDictionary<string, string>> types = new(() =>
        typeof(TypeFullNames).GetFields(BindingFlags.Public | BindingFlags.Static).
            ToImmutableDictionary(static x => x.GetValue(null).ToString(), static x => x.Name));

    string? IAttributeHandle.Write(AttributeHandleArguments args)
    {
        if (types.Value.TryGetValue(args.AttributeClassFullName, out var attrShortName))
        {
            if (args.ClassType != ClassType.Entities)
            {
                // 非实体类型不需要 EFCore 的特性
                if (args.AttributeClassFullName.StartsWith("Microsoft.EntityFrameworkCore."))
                    return args.AttributeClassFullName;
            }

            var template0 =
"""
    [
"""u8;
            args.Stream.Write(template0);
            args.Stream.Write(attrShortName);
            var value = args.Attribute.ToString()
                .Replace("System.ComponentModel.DataAnnotations.Schema.", string.Empty);
            var l_brace_index = value.IndexOf('(');
            if (l_brace_index >= 0)
            {
                var bytes = Encoding.UTF8.GetBytes(
                    value.ToCharArray(),
                    l_brace_index,
                    value.Length - l_brace_index);
                args.Stream.Write(bytes);
            }
            var template1 =
"""
]

"""u8;
            args.Stream.Write(template1);
            return args.AttributeClassFullName;
        }
        return default;
    }
}
