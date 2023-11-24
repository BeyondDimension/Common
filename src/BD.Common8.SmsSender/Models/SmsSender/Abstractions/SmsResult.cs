namespace BD.Common8.SmsSender.Models.SmsSender.Abstractions;

/// <summary>
/// 表示短信结果的抽象类
/// </summary>
/// <typeparam name="TImplement">实现类的类型，必须是 <see cref="SmsResult{TImplement}"/> 或其派生类</typeparam>
public abstract class SmsResult<TImplement> : JsonModel<TImplement>, IResult<ISmsSubResult?>
    where TImplement : SmsResult<TImplement>
{
    /// <summary>
    /// 获取或设置短信操作是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 获取或设置短信操作的结果信息
    /// </summary>
    public ISmsSubResult? Result { get; set; }

    /// <summary>
    /// 获取或设置短信操作的 HTTP 状态码
    /// </summary>
    public int HttpStatusCode { get; set; }
}

/// <summary>
/// 表示短信结果的泛型抽象类
/// </summary>
/// <typeparam name="TResult">结果的泛型参数类型，必须实现 <see cref="IJsonModel"/> 接口</typeparam>
/// <typeparam name="TImplement">实现类的类型，必须是 <see cref="SmsResult{TResult, TImplement}"/> 或其派生类</typeparam>
public abstract class SmsResult<TResult, TImplement> : SmsResult<TImplement>, ISmsResult, IResult<TResult?>
   where TResult : IJsonModel
   where TImplement : SmsResult<TResult, TImplement>
{
    /// <summary>
    /// 获取或设置短信操作的具体结果
    /// </summary>
    public new TResult? Result { get; set; }

    /// <summary>
    /// 获取或设置短信操作的结果对象
    /// </summary>
    public ISmsSubResult? ResultObject { get => base.Result; set => base.Result = value; }

    /// <summary>
    /// 将 <see cref="Result"/> 属性转换为 <see cref="ResultObject"/> 对象，并使用 <see cref="ISmsSubResult"/> 类型进行接收
    /// </summary>
    ISmsSubResult? ISmsResult.Result => ResultObject;
}