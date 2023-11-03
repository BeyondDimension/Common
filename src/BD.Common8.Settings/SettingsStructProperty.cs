namespace BD.Common8.Settings;

/// <summary>
/// 值类型的设置项属性
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TSettings"></typeparam>
/// <param name="default"></param>
/// <param name="autoSave"></param>
/// <param name="propertyName"></param>
public class SettingsStructProperty<TValue, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettings>(
    TValue? @default = default, bool autoSave = true, [CallerMemberName] string? propertyName = null) :
    SettingsStructPropertyBase<TValue, TSettings>(@default, autoSave, propertyName)
    where TValue : struct
{
    /// <summary>
    /// 获取或设置属性的值
    /// </summary>
    public TValue Value
    {
        get
        {
            var value = ActualValue;
            if (value.HasValue)
                return value.Value;
            return default;
        }

        set => ActualValue = value;
    }
}