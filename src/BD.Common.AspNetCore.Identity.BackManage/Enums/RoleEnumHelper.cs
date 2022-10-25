using static BD.Common.Enums.RoleEnumHelper;

// ReSharper disable once CheckNamespace
namespace BD.Common.Enums;

public static partial class RoleEnumHelper
{
    public const char SeparatorChar = ',';
    public const string SeparatorString = ", ";
    public const string Administrator = "Administrator";
    public const byte AdministratorValue = 1;

    static bool RoleEquals(string left, string right) => string.Equals(left, right);

    /// <summary>
    /// 判断权限集合中是否有指定的权限
    /// </summary>
    /// <param name="roles"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public static bool IsRole(IEnumerable<string>? roles, string roleName)
        => roles != null && roles.Any(x => RoleEquals(roleName, x));

    /// <summary>
    /// 在权限集合中添加指定的权限
    /// </summary>
    /// <param name="roles"></param>
    /// <param name="roleName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IList<string>? SetRole(IList<string>? roles, string roleName, bool value)
    {
        if (value)
        {
            if (roles == null)
                roles = new List<string> { roleName };
            else if (!roles.Any(x => RoleEquals(roleName, x)))
                roles.Add(roleName);
        }
        else
        {
            if (roles != null)
            {
                if (roles is List<string> roleList)
                {
                    roleList.RemoveAll(x => RoleEquals(roleName, x));
                }
                else
                {
                    roles = roles.Where(x => !RoleEquals(roleName, x)).ToList();
                }
            }
        }
        return roles;
    }

    public static Func<IEnumerable<string>?, string> ToDisplayStringDelegate { get; set; } = null!;

    /// <inheritdoc cref="RoleEnumHelper{TRoleEnum}.ToDisplayString(IEnumerable{string}?)"/>
    public static string ToDisplayString(IEnumerable<string>? roles) => ToDisplayStringDelegate(roles);

    public static Func<string?, IList<string>?> ParseDelegate { get; set; } = null!;

    /// <inheritdoc cref="RoleEnumHelper{TRoleEnum}.Parse(string?)"/>
    public static IList<string>? Parse(string? roles) => ParseDelegate(roles);
}

public static partial class RoleEnumHelper<TRoleEnum> where TRoleEnum : struct, Enum
{
    static readonly IReadOnlyDictionary<TRoleEnum, string> pros_map;
    static readonly ILookup<string, TRoleEnum> cons_map;

    static RoleEnumHelper()
    {
        pros_map = new Dictionary<TRoleEnum, string>()
        {
            { ConvertibleHelper.Convert<TRoleEnum, int>(AdministratorValue), "管理员" },
        };
        cons_map = pros_map.ToLookup(pair => pair.Value, pair => pair.Key);
    }

    /// <summary>
    /// 将权限枚举转换为显示字符串
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public static string ToDisplayString(TRoleEnum role)
        => pros_map.ContainsKey(role) ? pros_map[role] : role.ToString();

    static IEnumerable<TRoleEnum> Cast(IEnumerable<string> roles)
    {
        foreach (var role in roles)
        {
            if (Enum.TryParse<TRoleEnum>(role, out var value)) yield return value;
        }
    }

    /// <summary>
    /// 将权限集合转换为显示字符串
    /// </summary>
    /// <param name="roles"></param>
    /// <returns></returns>
    public static string ToDisplayString(IEnumerable<string>? roles)
    {
        if (roles == null || !roles.Any()) return string.Empty;
        roles = Cast(roles).Select(ToDisplayString);
        return string.Join(SeparatorString, roles);
    }

    /// <summary>
    /// 将显示字符串转换为权限集合
    /// </summary>
    /// <param name="roles"></param>
    /// <returns></returns>
    public static IList<string>? Parse(string? roles)
    {
        if (string.IsNullOrEmpty(roles)) return null;
        return Parse(roles.Split(SeparatorChar, StringSplitOptions.RemoveEmptyEntries));
    }

    static IList<string>? Parse(IEnumerable<string>? roles)
    {
        if (roles == null || !roles.Any()) return null;
        return ParseEnum(roles).Select(x => x.ToString()).ToList();
    }

    static IEnumerable<TRoleEnum> ParseEnum(IEnumerable<string> roles)
    {
        foreach (var role in roles)
        {
            if (Enum.TryParse<TRoleEnum>(role, out var value)) yield return value;
            if (cons_map.Contains(role)) yield return cons_map[role].First();
        }
    }
}