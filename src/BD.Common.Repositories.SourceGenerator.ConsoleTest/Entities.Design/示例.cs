#pragma warning disable IDE0044 // 添加只读修饰符
#pragma warning disable IDE0051 // 删除未使用的私有成员

namespace BD.Common.Repositories.SourceGenerator.ConsoleTest.Entities.Design;

[GenerateRepositories(
    RepositoryConstructorArguments = new[] {
        "ISysUserRepository",
        "ASDASDRepository",
    },
    ApiControllerConstructorArguments = new[] {
        "ISysMenuRepository",
    },
    ApiRoutePrefix = "ms/accelerator",
    DbContextBaseInterface = "IAcceleratorDbContext",
    BackManageUIPage = true
)]
/*[Table("TestXXXs")]*/ // 可使用 TableAttribute 指定表名称，不指定时将使用类名的复数单词
public class 示例
{
    Guid Id;

    Guid 租户Id;

    [BackManageField(Query = true)]
    string? Title;

    [BackManageField(Query = true)]
    bool 禁用;

    #region CloudFileInfo

    /// <summary>
    /// XXXX
    /// </summary>
    [Description("CloudFileInfo")]
    const PreprocessorDirective PD_CloudFileInfo_R = PreprocessorDirective.region;

    [MaxLength(255, ErrorMessage = "AAAA")]
    [Comment("xxxx")]
    [StringLength(35, ErrorMessage = "BBBB")]
    [BackManageField(Edit = true, Query = true)]
    string? 文件名;

    [MaxLength(Hashs.String.Lengths.SHA256 + 3)]
    [MinLength(Hashs.String.Lengths.SHA256)]
    string? SHA256;

    [MaxLength(Hashs.String.Lengths.SHA384 + 3)]
    [MinLength(Hashs.String.Lengths.SHA384)]
    string? SHA384;

    [Url]
    [MaxLength(MaxLengths.Url)]
    string? 文件路径;

    [MaxLength(MaxLengths.FileExtension)]
    string? 文件后缀名;

    byte 文件类型;

    [Range(0, int.MaxValue)]
    long 文件大小;

    [Range(0.5, int.MaxValue)]
    long 文件大小2;

    [Range(typeof(double), "0.25", "2555.2")]
    double 文件大小3;

    const PreprocessorDirective PD_CloudFileInfo_E = PreprocessorDirective.endregion;

    #endregion

    [BackManageField(Edit = true, Query = true)]
    [MaxLength(MaxLengths.Url)]
    string? 访问地址;

    bool 是否删除;

    [BackManageField(Query = true)]
    Guid? 创建人;

    [BackManageField(Edit = true, Query = true)]
    DateTimeOffset 创建时间;

#if !TEST_IF_XXXXXXXXXX

    [BackManageField(Query = true)]
    Guid? 操作人;

#elif !TEST_IF_XXXX

#else

#endif

    DateTimeOffset 更新时间;

    public string? A { get; set; } // 随便定义一个属性，干扰源生成，需要忽略无关的数据

    static void B()
    {

    }

    static Task C() => Task.CompletedTask;

    const int TestConst = 1;

    [BackManageField(Edit = true, Query = true)]
    [Required]
    [StringLength(35, MinimumLength = 1, ErrorMessage = "D")]
    string 名称 = "";

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    Guid Key;

    [BackManageField(Edit = true)]
    [Precision(5, 2)]
    float 平均分;

    [BackManageField(Edit = true)]
    [Precision(5)]
    float 平均分2;

    [BackManageField(Edit = true, Query = true)]
    [EmailAddress]
    string 邮箱 = "";

    [BackManageField(Edit = true, Query = true)]
    Architecture Architecture = Architecture.X64;

    [BackManageField(Edit = true)]
    string? Describe;

    [BackManageField(Edit = true)]
    string? DisableReason;

    [BackManageField(Edit = true, Query = true)]
    Gender Gender;

    [BackManageField(Edit = true, Query = true)]
    string? NickName;

    [BackManageField(Edit = true)]
    long Order;

    [BackManageField(Edit = true)]
    bool IsTop;

    [BackManageField(Edit = true)]
    string? IPAddress;

    [BackManageField(Edit = true)]
    string? Password;

    [BackManageField(Edit = true)]
    string? Remarks;

    [BackManageField(Edit = true)]
    string? SmsCode;

    [Range(0, double.PositiveInfinity)]
    double D;

    [Range(0, float.PositiveInfinity)]
    float F;

    [Range(0, double.NegativeInfinity)]
    double D1;

    [Range(0, float.NegativeInfinity)]
    float F2;

    [Range(0, double.NegativeInfinity)]
    decimal DE;

    #region TODO：可选的仅用于表格查询参数中的字段，并定义查询表达式

    #endregion

    #region TODO：可选的仅用于表格详情只读中的字段，并定义查询与设置值表达式

    #endregion
}
