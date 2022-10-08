namespace BD.Common.Models.Abstractions;

public interface IJsonModel
{

}

public abstract class JsonModel : IJsonModel
{
    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<挂起>")]
    public override string? ToString()
    {
        try
        {
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            return JsonSerializer.Serialize(this, GetType(), options: JsonSerializerCompatOptions.WriteIndented);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        }
        catch
        {
            return base.ToString();
        }
    }
}

public abstract class JsonModel<T> : IJsonModel where T : JsonModel<T>
{
    public override string? ToString()
    {
        try
        {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            return JsonSerializer.Serialize<T>((T)this, options: JsonSerializerCompatOptions.WriteIndented);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        }
        catch
        {
            return base.ToString();
        }
    }
}