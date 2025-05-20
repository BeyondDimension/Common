using System.Diagnostics.CodeAnalysis;

namespace BD.Common8.UserInput.ModelValidator.Services;

/// <summary>
/// 模型验证
/// </summary>
public interface IModelValidator
{
    /// <summary>
    /// 验证模型，返回结果以及错误消息
    /// </summary>
    /// <param name="model"></param>
    /// <param name="errorMessage"></param>
    /// <param name="ignores"></param>
    /// <returns></returns>
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    bool Validate(object model, [NotNullWhen(false)] out string? errorMessage, params Type[] ignores);

    /// <inheritdoc cref="Validate(object, out string?, Type[])"/>
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public bool Validate(object model, params Type[] ignores) => Validate(model, out var _, ignores);
}