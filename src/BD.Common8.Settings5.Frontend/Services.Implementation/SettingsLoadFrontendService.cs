using static BD.Common8.Settings5.Services.ISettingsLoadService;

namespace BD.Common8.Settings5.Services.Implementation;

/// <summary>
/// <see cref="ISettingsLoadService"/> 的前端(Frontend)实现
/// </summary>
public sealed class SettingsLoadFrontendService : ISettingsLoadService
{
    /// <inheritdoc/>
    public bool FrontendLoad<[DynamicallyAccessedMembers(DAMT_M)] TSettingsModel>(
        out Action<IServiceCollection>? configureServices,
        out IOptionsMonitor<TSettingsModel>? options) where TSettingsModel : class, new()
    {
        options = default;
        configureServices = default;

        // TODO
        return false;
    }

    /// <inheritdoc/>
    public bool BackendLoad<[DynamicallyAccessedMembers(DAMT_M)] TSettingsModel>(
        out Action<IServiceCollection>? configureServices,
        out IOptionsMonitor<TSettingsModel>? options,
        bool settingsFileDirectoryExists,
        string? settingsFileDirectory = null) where TSettingsModel : class, new()
        => FrontendLoad(out configureServices, out options);

    /// <inheritdoc/>
    public void Save<[DynamicallyAccessedMembers(DAMT_M)] TSettingsModel>(TSettingsModel settingsModel, bool force = true) where TSettingsModel : class, new()
    {
        throw new NotImplementedException();
    }
}
