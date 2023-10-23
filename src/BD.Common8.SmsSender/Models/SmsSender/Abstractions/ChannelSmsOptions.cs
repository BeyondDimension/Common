namespace BD.Common8.SmsSender.Models.SmsSender.Abstractions;

#pragma warning disable SA1600 // Elements should be documented

public abstract class ChannelSmsOptions : JsonModel, IDisable, IExplicitHasValue
{
    public bool Disable { get; set; }

    public virtual bool IsValid() => !Disable;

    bool IExplicitHasValue.ExplicitHasValue() => IsValid();
}
