//namespace BD.Common8.Essentials.Services.Implementation;

//partial class PreferencesPlatformServiceImpl
//{
//    readonly SQLiteConnection conn;

//    /// <summary>
//    /// 初始化 <see cref="PreferencesPlatformServiceImpl"/> 类的新实例
//    /// </summary>
//    public PreferencesPlatformServiceImpl()
//    {
//        conn = Repository.GetDbConnectionSync<Entity>();
//    }

//    /// <summary>
//    /// 根据给定的键返回对应的 Id 值，并附加 "_K" 后缀
//    /// </summary>
//    internal static string GetId(string key) => $"{key}_K";

//    /// <summary>
//    /// 根据给定的键和共享名称，返回对应的 Id 值。
//    /// 如果共享名称为空，则调用 <see cref="GetId(string)"/> 方法获取 Id 值
//    /// 如果共享名称不为空，则在键和共享名称之间用 "_" 连接，附加 "_S" 后缀
//    /// </summary>
//    internal static string GetId(string key, string? sharedName)
//    {
//        if (sharedName == null) return GetId(key);
//        return $"{key}_{sharedName}_S";
//    }

//    /// <summary>
//    /// 尝试执行指定的委托方法，并在遇到异常时进行重试
//    /// </summary>
//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    static T AttemptAndRetry<T>(Func<T> @delegate,
//        int retryCount = Repository.DefaultRetryCount)
//        => Policy.Handle<SQLiteException>()
//        .WaitAndRetry(retryCount, Repository.PollyRetryAttempt)
//        .Execute(@delegate);

//    /// <summary>
//    /// 清除指定共享名称的平台数据
//    /// </summary>
//    public void PlatformClear(string? sharedName)
//    {
//        AttemptAndRetry(() =>
//        {
//            var r = conn.Execute(Sql_Delete_Where_SharedName_Equals, sharedName ?? string.Empty);
//            return r;
//        });
//    }

//    /// <summary>
//    /// 检查指定键和共享名称的平台数据是否存在
//    /// </summary>
//    public bool PlatformContainsKey(string key, string? sharedName)
//    {
//        var id = GetId(key, sharedName);
//        sharedName ??= string.Empty;
//        return AttemptAndRetry(() =>
//        {
//            var item = conn.Table<Entity>()
//                .Where(x => x.Id == id && x.SharedName == sharedName)
//                .FirstOrDefault();
//            return item != null;
//        });
//    }

//    /// <summary>
//    /// 获取指定键和共享名称的平台数据，如果数据不存在，则返回 defaultValue 参数指定的默认值
//    /// 数据的类型由泛型参数 T 指定，该类型必须实现 IConvertible 接口
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="key"></param>
//    /// <param name="defaultValue"></param>
//    /// <param name="sharedName"></param>
//    /// <returns></returns>
//    public T? PlatformGet<T>(string key, T? defaultValue, string? sharedName) where T : notnull, IConvertible
//    {
//        var id = GetId(key, sharedName);
//        sharedName ??= string.Empty;
//        return AttemptAndRetry(() =>
//        {
//            var item = conn.Table<Entity>()
//                .Where(x => x.Id == id && x.SharedName == sharedName)
//                .FirstOrDefault();
//            if (item == null)
//                return defaultValue;
//            var value = Convert2.Convert<T>(item.Value);
//            return value;
//        });
//    }

//    /// <summary>
//    /// 移除指定键和共享名称的平台数据
//    /// </summary>
//    public void PlatformRemove(string key, string? sharedName)
//    {
//        var id = GetId(key, sharedName);
//        AttemptAndRetry(() =>
//        {
//            var r = conn.Delete<Entity>(id);
//            return r;
//        });
//    }

//    /// <summary>
//    /// 根据指定的键名和共享名称在数据库中设置平台首选项的值
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="key"></param>
//    /// <param name="value"></param>
//    /// <param name="sharedName"></param>
//    public void PlatformSet<T>(string key, T? value, string? sharedName) where T : notnull, IConvertible
//    {
//        if (value == null)
//        {
//            PlatformRemove(key, sharedName);
//        }
//        else
//        {
//            var id = GetId(key, sharedName);
//            AttemptAndRetry(() =>
//            {
//                var r = conn.InsertOrReplace(new Entity
//                {
//                    Id = id,
//                    Value = value.ToString(CultureInfo.InvariantCulture),
//                    SharedName = sharedName ?? string.Empty,
//                });
//                return r;
//            });
//        }
//    }

//    /// <summary>
//    /// 删除符合指定共享名称条件的记录
//    /// </summary>
//    const string Sql_Delete_Where_SharedName_Equals =
//        $"DELETE FROM \"{TableName}\" WHERE \"{ColumnName_SharedName}\" = ?";

//    //const string Sql_Delete_Where_Id_Equals_And_SharedName_IsNull =
//    //    $"DELETE FROM \"{TableName}\" WHERE \"{ColumnName_Id}\" = ? AND \"{ColumnName_SharedName}\" = NULL";

//    //const string Sql_Delete_Where_Id_Equals_And_SharedName_Equals =
//    //    $"DELETE FROM \"{TableName}\" WHERE \"{ColumnName_Id}\" = ? AND \"{ColumnName_SharedName}\" = ?";

//    //const string Sql_Select_SharedName_Where_Key_Equals_And_SharedName_IsNull =
//    //    $"SELECT \"{ColumnName_SharedName}\" FROM \"{TableName}\" WHERE \"{ColumnName_Id}\" = ? AND \"{ColumnName_SharedName}\" = NULL";

//    //const string Sql_Select_SharedName_Where_Key_Equals_And_SharedName_Equals =
//    //    $"SELECT \"{ColumnName_SharedName}\" FROM \"{TableName}\" WHERE \"{ColumnName_Id}\" = ? AND \"{ColumnName_SharedName}\" = ?";

//    //const string Sql_Select_Value_Where_Key_Equals_And_SharedName_IsNull =
//    //    $"SELECT \"{ColumnName_Value}\" FROM \"{TableName}\" WHERE \"{ColumnName_Id}\" = ? AND \"{ColumnName_SharedName}\" = NULL";

//    //const string Sql_Select_Value_Where_Key_Equals_And_SharedName_Equals =
//    //    $"SELECT \"{ColumnName_Value}\" FROM \"{TableName}\" WHERE \"{ColumnName_Id}\" = ? AND \"{ColumnName_SharedName}\" = ?";
//}
