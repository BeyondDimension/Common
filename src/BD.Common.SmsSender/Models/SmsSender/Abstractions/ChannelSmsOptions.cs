namespace BD.Common.Models.SmsSender.Abstractions;

public abstract class ChannelSmsOptions : JsonModel, IDisable, IExplicitHasValue
{
    public bool Disable { get; set; }

    public virtual bool IsValid() => !Disable;

    bool IExplicitHasValue.ExplicitHasValue() => IsValid();
}
