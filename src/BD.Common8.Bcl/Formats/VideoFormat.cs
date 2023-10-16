namespace System.Formats;

/// <summary>
/// 视频格式
/// </summary>
public enum VideoFormat : byte
{
    /// <summary>
    /// MPEG-4 Video Elemental Stream（MPEG-4 视频元素流）
    /// </summary>
    MP4 = 1,

    /// <summary>
    /// Web Media, Chrome 6, Edge 173 (desktop only), Firefox 4, Opera 10.6, Safari (WebRTC only)
    /// </summary>
    WebM,
}