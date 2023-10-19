namespace BD.Common8.Primitives.Models.Abstractions;

#pragma warning disable SA1600 // Elements should be documented

public interface IJsonModel
{
}

public abstract class JsonModel : IJsonModel
{
    public override string? ToString()
    {
        try
        {
            return SystemTextJsonSerializer.Serialize(this, GetType(), options: JsonSerializerCompatOptions.WriteIndented);
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
            return SystemTextJsonSerializer.Serialize<T>((T)this, options: JsonSerializerCompatOptions.WriteIndented);
        }
        catch
        {
            return base.ToString();
        }
    }
}