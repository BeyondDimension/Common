namespace BD.Common.Models.Abstractions;

public abstract class JsonModel
{
    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<挂起>")]
    public override string? ToString()
    {
        try
        {
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            return JsonSerializer.Serialize(this, GetType(), options: new()
            {
                WriteIndented = true,
            });
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        }
        catch
        {
            return base.ToString();
        }
    }
}
