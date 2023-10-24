namespace BD.Common8.Essentials.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

partial class PreferencesPlatformServiceImpl
{
    readonly SQLiteConnection conn;

    /// <summary>
    /// Initializes a new instance of the <see cref="PreferencesPlatformServiceImpl"/> class.
    /// </summary>
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T AttemptAndRetry<T>(Func<T> @delegate,
        int retryCount = Repository.DefaultRetryCount)
        => Policy.Handle<SQLiteException>()
        .WaitAndRetry(retryCount, Repository.PollyRetryAttempt)
        .Execute(@delegate);

    public void PlatformClear(string? sharedName)
    {
        AttemptAndRetry(() =>
        {
            var r = conn.Execute(Sql_Delete_Where_SharedName_Equals, sharedName ?? string.Empty);
            return r;
        });
    }

    public bool PlatformContainsKey(string key, string? sharedName)
    {
        var id = GetId(key, sharedName);
        sharedName ??= string.Empty;
        return AttemptAndRetry(() =>
        {
            var item = conn.Table<Entity>()
                .Where(x => x.Id == id && x.SharedName == sharedName)
                .FirstOrDefault();
            return item != null;
        });
    }

    public T? PlatformGet<T>(string key, T? defaultValue, string? sharedName) where T : notnull, IConvertible
    {
        var id = GetId(key, sharedName);
        sharedName ??= string.Empty;
        return AttemptAndRetry(() =>
        {
            var item = conn.Table<Entity>()
                .Where(x => x.Id == id && x.SharedName == sharedName)
                .FirstOrDefault();
            if (item == null)
                return defaultValue;
            var value = Convert2.Convert<T>(item.Value);
            return value;
        });
    }

    public void PlatformRemove(string key, string? sharedName)
    {
        var id = GetId(key, sharedName);
        AttemptAndRetry(() =>
        {
            var r = conn.Delete<Entity>(id);
            return r;
        });
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
            AttemptAndRetry(() =>
            {
                var r = conn.InsertOrReplace(new Entity
                {
                    Id = id,
                    Value = value.ToString(CultureInfo.InvariantCulture),
                    SharedName = sharedName ?? string.Empty,
                });
                return r;
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
