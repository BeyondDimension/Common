# [Steamworks SDK](https://partner.steamgames.com/doc/sdk)
Steamworks SDK 提供了一系列功能，旨在帮助您高效地在 Steam 上发布应用程序或游戏。  
Steamworks SDK 仅要求将[您的内容上传至 Steam](https://partner.steamgames.com/doc/sdk/uploading)，其他任何功能都是可选的。

#### 最新的 Steamworks SDK
您可在[此处](https://partner.steamgames.com/downloads/steamworks_sdk.zip)下载最新版的 Steamworks SDK。

#### 入门指南
SDK 的完整功能列表如下所示：
- glmgr - “ToGL”，适用于 macOS 的 DirectX to OpenGL 兼容性层。 参见 [ToGL github](https://github.com/ValveSoftware/ToGL) 页面，了解更多信息。
- public/steam - [Steamworks API 概览](https://partner.steamgames.com/doc/sdk/api)
- redistributable_bin - Steamworks API 可再发行二进制文件（请见上文的 Steamworks API 概览。）
  - 已打包为 [NuGet 本机资产包](https://learn.microsoft.com/zh-cn/nuget/create-packages/native-files-in-net-packages#native-assets) [![NuGet](https://img.shields.io/nuget/v/BD.Common8.3rdParty.Facepunch.Steamworks.NativeAssets.svg)](https://www.nuget.org/packages/BD.Common8.3rdParty.Facepunch.Steamworks.NativeAssets)
- steamworksexample - [Steamworks API 示例应用程序（SpaceWar）](https://partner.steamgames.com/doc/sdk/api/example)
- 工具
  - ContentBuilder - [上传至 Steam](https://partner.steamgames.com/doc/sdk/uploading)
  - ContentServer - [SteamPipe 本地内容服务器](https://partner.steamgames.com/doc/sdk/uploading/local_content_server)
  - drm - [Steam DRM](https://partner.steamgames.com/doc/features/drm)
  - goldmaster - [创建零售及“Gold Master”磁盘](https://partner.steamgames.com/doc/sdk/goldmaster)
  - Linux - 生成版本说明。 参见：[基于 SteamOS 和 Linux 的开发](https://partner.steamgames.com/doc/store/application/platforms/linux)
  - ContentPrep.zip - 已弃用。 在 SteamPipe 推出前，曾用于设置 macOS 应用程序的正确权限。
  - SteamPipeGUI.zip - 适用于 Windows 平台的 [SteamPipe GUI Tool](https://partner.steamgames.com/doc/sdk/uploading#steampipe_gui_tool)，让上传简单产品变得更轻松。

旧版本的 Steamworks SDK 可能包含其他一些已不再使用的工具。