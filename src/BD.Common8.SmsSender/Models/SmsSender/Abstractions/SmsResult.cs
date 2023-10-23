namespace BD.Common8.SmsSender.Models.SmsSender.Abstractions;

#pragma warning disable SA1600 // Elements should be documented

public abstract class SmsResult<TImplement> : JsonModel<TImplement>, IResult<ISmsSubResult?>
    where TImplement : SmsResult<TImplement>
{
    public bool IsSuccess { get; set; }

    public ISmsSubResult? Result { get; set; }

    public int HttpStatusCode { get; set; }
}

public abstract class SmsResult<TResult, TImplement> : SmsResult<TImplement>, ISmsResult, IResult<TResult?>
   where TResult : IJsonModel
   where TImplement : SmsResult<TResult, TImplement>
{
    public new TResult? Result { get; set; }

    public ISmsSubResult? ResultObject { get => base.Result; set => base.Result = value; }

    ISmsSubResult? ISmsResult.Result => ResultObject;
}