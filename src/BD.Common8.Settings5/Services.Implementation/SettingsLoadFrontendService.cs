using static BD.Common8.Settings5.Services.ISettingsLoadService;

namespace BD.Common8.Settings5.Services.Implementation;

/// <summary>
/// <see cref="ISettingsLoadService"/> 的前端(Frontend)实现
/// </summary>
public sealed class SettingsLoadFrontendService : ISettingsLoadService
{
    /// <inheritdoc/>
    public bool Load<[DynamicallyAccessedMembers(DAMT_M)] TSettingsModel>(
        out Action<IServiceCollection>? configureServices,
        out IOptionsMonitor<TSettingsModel>? options,
        bool settingsFileDirectoryExists,
        string? settingsFileDirectory = null) where TSettingsModel : class, new()
    {
        options = default;
        configureServices = default;

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Save<[DynamicallyAccessedMembers(DAMT_M)] TSettingsModel>(TSettingsModel settingsModel, bool force = true) where TSettingsModel : class, new()
    {
        throw new NotImplementedException();
    }
}
