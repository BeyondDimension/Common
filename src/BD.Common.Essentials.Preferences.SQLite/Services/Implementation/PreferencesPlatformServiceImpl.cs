using SQLite;

namespace BD.Common.Services.Implementation;

partial class PreferencesPlatformServiceImpl
{
    readonly SQLiteConnection conn;

    public PreferencesPlatformServiceImpl()
    {
        conn = Repository.GetDbConnectionSync<Entity>();
    }

    internal static string GetId(string key) => $"{key}_K";

    internal static string GetId(string key, string? sharedName)
    {
        if (sharedName == null) return GetId(key);
        return $"{key}_{sharedName}_S";
    }

    public void PlatformClear(string? sharedName)
    {
        conn.Execute(Sql_Delete_Where_SharedName_Equals, sharedName ?? string.Empty);
    }

    public bool PlatformContainsKey(string key, string? sharedName)
    {
        var id = GetId(key, sharedName);
        sharedName ??= string.Empty;
        var item = conn.Table<Entity>()
            .Where(x => x.Id == id && x.SharedName == sharedName)
            .FirstOrDefault();
        return item != null;
    }

    public T? PlatformGet<T>(string key, T? defaultValue, string? sharedName) where T : notnull, IConvertible
    {
        var id = GetId(key, sharedName);
        sharedName ??= string.Empty;
        var item = conn.Table<Entity>()
            .Where(x => x.Id == id && x.SharedName == sharedName)
            .FirstOrDefault();
        if (item == null) return defaultValue;
        var value = ConvertibleHelper.Convert<T>(item.Value);
        return value;
    }

    public void PlatformRemove(string key, string? sharedName)
    {
        var id = GetId(key, sharedName);
        conn.Delete<Entity>(id);
    }

    public void PlatformSet<T>(string key, T? value, string? sharedName) where T : notnull, IConvertible
    {
        if (value == null)
        {
            PlatformRemove(key, sharedName);
        }
        else
        {
            var id = GetId(key, sharedName);
            conn.InsertOrReplace(new Entity
            {
                Id = id,
                Value = value.ToString(CultureInfo.InvariantCulture),
                SharedName = sharedName ?? string.Empty,
            });
        }
    }

    const string Sql_Delete_Where_SharedName_Equals =
        $"DELETE FROM \"{TableName}\" WHERE \"{ColumnName_SharedName}\" = ?";

    //const string Sql_Delete_Where_Id_Equals_And_SharedName_IsNull =
    //    $"DELETE FROM \"{TableName}\" WHERE \"{ColumnName_Id}\" = ? AND \"{ColumnName_SharedName}\" = NULL";

    //const string Sql_Delete_Where_Id_Equals_And_SharedName_Equals =
    //    $"DELETE FROM \"{TableName}\" WHERE \"{ColumnName_Id}\" = ? AND \"{ColumnName_SharedName}\" = ?";

    //const string Sql_Select_SharedName_Where_Key_Equals_And_SharedName_IsNull =
    //    $"SELECT \"{ColumnName_SharedName}\" FROM \"{TableName}\" WHERE \"{ColumnName_Id}\" = ? AND \"{ColumnName_SharedName}\" = NULL";

    //const string Sql_Select_SharedName_Where_Key_Equals_And_SharedName_Equals =
    //    $"SELECT \"{ColumnName_SharedName}\" FROM \"{TableName}\" WHERE \"{ColumnName_Id}\" = ? AND \"{ColumnName_SharedName}\" = ?";

    //const string Sql_Select_Value_Where_Key_Equals_And_SharedName_IsNull =
    //    $"SELECT \"{ColumnName_Value}\" FROM \"{TableName}\" WHERE \"{ColumnName_Id}\" = ? AND \"{ColumnName_SharedName}\" = NULL";

    //const string Sql_Select_Value_Where_Key_Equals_And_SharedName_Equals =
    //    $"SELECT \"{ColumnName_Value}\" FROM \"{TableName}\" WHERE \"{ColumnName_Id}\" = ? AND \"{ColumnName_SharedName}\" = ?";
}
