// ReSharper disable once CheckNamespace
namespace BD.Common.Models.Abstractions;

#if !BLAZOR
public abstract partial class AppSettingsBase : INotUseForwardedHeaders
{
    public bool NotUseForwardedHeaders { get; set; }

    [IgnoreDataMember]
    [JsonIgnore]
    public SigningCredentials? SigningCredentials { get; set; }
}
#endif