namespace BD.Common.Services.Implementation;

/// <inheritdoc cref="IModelValidator"/>
public sealed class ModelValidator : IModelValidator
{
    static readonly IDictionary<Type, Func<object, string?>> validators = new Dictionary<Type, Func<object, string?>>();

    public static void AddColumnValidate<T>(Func<T, string?> validate)
        => validators.TryAdd(typeof(T), model => model is T value ? validate(value) : null);

    const string I = "I";
    const string IReadOnly = "IReadOnly";
    const string IReadOnlyNullable = "IReadOnlyNullable";
    static readonly string[] StartsWithSubstrings = new[] { IReadOnlyNullable, IReadOnly, I };

    static ReadOnlySpan<char> GetColumnName(Type type)
    {
        foreach (var item in StartsWithSubstrings)
        {
            if (type.Name.StartsWith(item))
            {
                return type.Name.AsSpan(item.Length);
            }
        }
        return type.Name;
    }

    public bool Validate(object model, [NotNullWhen(false)] out string? errorMessage, params Type[] ignores)
    {
        var hasIgnores = ignores.Any_Nullable();
        if (!validators.Any())
        {
            // 验证组不能为空集合！
            throw new NotSupportedException("validators cannot be empty.");
        }
        foreach (var item in validators)
        {
            if (hasIgnores)
            {
                var ignored = false;
                foreach (var ignore in ignores)
                {
                    if (ignore == item.Key || GetColumnName(ignore).SequenceEqual(GetColumnName(item.Key)))
                    {
                        ignored = true;
                        break;
                    }
                }
                if (ignored) continue;
            }
            errorMessage = item.Value(model);
            if (errorMessage != null) return false;
        }

        errorMessage = null;
        return true;
    }
}