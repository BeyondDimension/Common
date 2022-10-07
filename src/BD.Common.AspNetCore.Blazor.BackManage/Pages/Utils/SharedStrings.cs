using static BD.Common.Pages.Utils.SharedStrings;
using EGender = BD.Common.Enums.Gender;

// ReSharper disable once CheckNamespace
namespace BD.Common.Pages.Utils;

public abstract class SharedStrings
{
    protected SharedStrings() => throw new InvalidOperationException();

    public const string Key = "主键";
    public const string Title = "标题";
    public const string State = "状态";
    public const string Action = "操作";
    public const string StateDisableTrue = "已禁用";
    public const string StateDisableFalse = "已启用";
    public const string DisableTrue = "禁用";
    public const string DisableFalse = "启用";
    public const string DisableReason = "禁用原因";
    public const string PleaseEnterDisableReason = "请输入禁用原因";
    public const string MoveUp = "上移";
    public const string MoveDown = "下移";
    public const string Order = "排序";
    public const string None = "无";
    public const string Query = "查询";
    public const string Add = "添加";
    public const string AddSuccess = "添加成功";
    public const string Edit = "修改";
    public const string Save = "保存";
    public const string EditSuccess = "修改成功";
    public const string PleaseEnter = "请输入";
    public const string PleaseSelect = "请选择";
    public const string Ok = "确定";
    public const string Cancel = "取消";
    public const string Close = "关闭";
    public const string Detail = "详情";
    public const string Type = "类型";
    public const string Date = "日期";
    public const string PublishUser = "发布人";
    public const string NickName = "昵称";
    public const string CreationTime = "创建时间";
    public const string UpdateTime = "更新时间";
    public const string Delete = "删除";
    public const string DeleteSuccess = "删除成功";
    public const string All = "全部";
    public const string Warning = "警告";
    public const string ConfirmContentDelete_ = "是否确认删除 {0} ？";
    public const string Remark = "备注";
    public const string LockoutUser = "用户编辑锁";
    public const string LockoutEnd = "锁定截止时间";
    public const string Unlock = "解锁";
    public const string UnlockSuccess = "解锁成功";
    public const string Gender = "性别";
    public const string GenderUnkown = "未知";
    public const string GenderMale = "男";
    public const string GenderFemale = "女";
    public const string UserName = "用户名";
    public const string Submit = "提交";
    public const string Avatar = "头像";
    public const string ChangeAvatar = "更换头像";
    public const string Modify = "修改";
    public const string ChangePassword = "修改密码";
    public const string Password = "密码";
    public const string OldPassword = "旧密码";
    public const string NewPassword = "新密码";
    public const string PleaseEnterPassword = "请输入密码";
    public const string PleaseEnterPassword2 = "请再次输入密码";
    public const string PleaseEnterOldPassword = "请输入旧密码";
    public const string PleaseEnterNewPassword = "请输入新密码";
    public const string PleaseEnterNewPasswordConfirm = "请再次输入新密码";
    public const string Download_ = "下载 {0}";
    public const string Upload = "上传";
    public const string Download = "下载";
    public const string Roles = "权限";
    public const string Subcategory = "子分类";

    public const string OperatorUser = "操作人";
    public const string PleaseEnterOperatorUser = "请输入操作人";
    public const string CreateUser = "创建人";
    public const string PleaseEnterCreateUser = "请输入创建人";
    public const string Name = "名称";
    public const string PleaseEnterName = "请输入名称";
    public const string Quantity = "数量";
    public const string PleaseEnterQuantity = "请输入数量";

    public const string IsTop = "是否置顶";

    public const string Yes = "是";
    public const string No = "否";

    public const string Email = "邮箱";
    public const string PleaseEnterEmail = "请输入邮箱";

    public const string SuccessCount = "成功次数";
    public const string FailCount = "失败次数";

    public const string AddSingle = "添加单条";
    public const string BatchGenerate = "批量生成";
    public const string BatchImport = "批量导入";

    public static string Format(string s, params object?[] args)
    {
        try
        {
            return string.Format(s, args);
        }
        catch
        {
            StringBuilder b = new(s);
            Array.ForEach(args, x =>
            {
                b.Append(", ");
                b.Append(x);
            });
            return b.ToString();
        }
    }

    public const string True = "true";
    public const string False = "false";

    public const string ImageFallback = "data:image/png;base64,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";

    public const string ImageFallback43 = "/img/img-placeholder-4-3.webp";

    public const string ColorGrey = "#d9d9d9";
    public const string ColorPink = "#eb2f96";
    public const string ColorBlue = "#1890ff";
    public const string ColorOrange = "#fa8c16";
    public const string ColorGreen = "#52c41a";
    public const string ColorYellow = "#fadb14";
    public const string ColorRed = "#f5222d";
}

public static partial class Extensions
{
    public static string ToLowerString(this bool value) => value ? True : False;

    public static PresetColor ToPresetColor(this IDisable m) => m.Disable ? PresetColor.Red : PresetColor.Green;

    public static string ToColor(this Gender m) => m switch
    {
        EGender.Male => ColorBlue,
        EGender.Female => ColorPink,
        _ => ColorOrange,
    };

    public static string ToColor(this Gender? m) => m.HasValue ? m.Value.ToColor() : ColorGrey;

    public static string ToDisplayString(this Gender m) => m switch
    {
        EGender.Male => GenderMale,
        EGender.Female => GenderFemale,
        _ => GenderUnkown,
    };

    public static string ToStatus<TPrimaryKey>(this IDisableKeyModel<TPrimaryKey> m)
        where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
        => EqualityComparer<TPrimaryKey>.Default.Equals(m.Id, default) ? "default" : m.Disable ? "error" : "success";

    public static string ToDisableStateString(this IDisable m) => m.Disable ? StateDisableTrue : StateDisableFalse;

    public static string ToDisableActionButtonString(this IDisable m) => m.Disable ? DisableFalse : DisableTrue;

    public const string IsTopABSTrue = "优先";
    public const string IsTopABSFalse = "取消";

    public static string ToIsTopActionButtonString(this IIsTop m) => !m.IsTop ? IsTopABSTrue : IsTopABSFalse;

    public static string ToIsTopActionButtonType(this IIsTop m) => !m.IsTop ? ButtonType.Primary : ButtonType.Default;
}
