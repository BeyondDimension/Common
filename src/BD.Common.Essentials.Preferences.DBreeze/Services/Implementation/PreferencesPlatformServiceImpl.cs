using BD.Common.Settings;
using SQLite;

namespace BD.Common.Services.Implementation;

partial class PreferencesPlatformServiceImpl
{
    static string GetTableName(string? sharedName) => $"Preferences.{sharedName}";

    public void PlatformSet<T>(string key, T? value, string? sharedName) where T : notnull, IConvertible
    {
        if (value == null)
        {
            PlatformRemove(key, sharedName);
        }
        else
        {
            using var t = SettingsProviderV3.Provider.GetTransaction();
            t.Insert(GetTableName(sharedName), key, value.ToString(CultureInfo.InvariantCulture));
            t.Commit();
        }
    }

    public void PlatformClear(string? sharedName)
    {
        using var t = SettingsProviderV3.Provider.GetTransaction();
        t.RemoveAllKeys(GetTableName(sharedName), true);
        t.Commit();
    }

    public bool PlatformContainsKey(string key, string? sharedName)
    {
        using var t = SettingsProviderV3.Provider.GetTransaction();
        var row = t.Select<string, string>(GetTableName(sharedName), key);
        if (row == null) return false;
        return row.Exists;
    }

    public T? PlatformGet<T>(string key, T? defaultValue, string? sharedName) where T : notnull, IConvertible
    {
        using var t = SettingsProviderV3.Provider.GetTransaction();
        var row = t.Select<string, string>(GetTableName(sharedName), key);
        if (row == null || !row.Exists) return defaultValue;
        var value = ConvertibleHelper.Convert<T>(row.Value);
        return value;
    }

    public void PlatformRemove(string key, string? sharedName)
    {
        using var t = SettingsProviderV3.Provider.GetTransaction();
        t.RemoveKey(GetTableName(sharedName), key);
        t.Commit();
    }

    /// <summary>
    /// 将数据从
    /// <para>使用 SQLiteConnection 的 PreferencesPlatformServiceImpl</para>
    /// <para>迁移到</para>
    /// <para>使用 DBreeze 的 PreferencesPlatformServiceImpl</para>
    /// </summary>
    public static void Migrate()
    {
        var dbPath = Repository.DataBaseDirectory;
        bool isOK = false;
        bool exists = false;
        try
        {
            var dbFilePath = Path.Combine(dbPath, "application2.dbf");
            try
            {
                exists = File.Exists(dbFilePath);
            }
            catch
            {

            }
            if (!exists) return;
            var conn = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            var items = conn.Table<Entity>().ToArray();
            if (items.Any())
            {
                var itemGroups = items.GroupBy(x => x.SharedName).ToArray();
                foreach (var itemGroup in itemGroups)
                {
                    using var t = SettingsProviderV3.Provider.GetTransaction();
                    foreach (var item in itemGroup)
                    {
                        if (item.Value != null)
                        {
                            string key;
                            if (string.IsNullOrEmpty(item.SharedName)) key = item.Id.TrimEnd("_K");
                            else key = item.Id.TrimEnd($"_{item.SharedName}_S");
                            var tableName = GetTableName(item.SharedName);
                            t.Insert(tableName, key, item.Value.ToString(CultureInfo.InvariantCulture));
                        }
                    }
                    t.Commit();
                }
            }
            conn.Close();
            conn.Dispose();
            isOK = true;
        }
        catch
        {

        }
        finally
        {
            if (isOK)
            {
                var files = Directory.GetFiles(dbPath, "application2*");
                foreach (var item in files)
                {
                    IOPath.FileTryDelete(item);
                }
            }
        }
    }
}
