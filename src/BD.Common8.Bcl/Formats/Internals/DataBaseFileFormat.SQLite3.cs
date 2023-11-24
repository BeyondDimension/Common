namespace System.Formats.Internals;

/// <summary>
/// 数据库文件格式
/// </summary>
partial class DataBaseFileFormat
{
    /// <summary>
    /// SQLite3 数据库文件格式
    /// </summary>
    public static class SQLite3
    {
        /// <summary>
        /// 用于识别 SQLite3 数据库文件的幻数
        /// </summary>
        public static readonly byte[] MagicNumber = [83, 81, 76, 105, 116, 101, 32, 102, 111, 114, 109, 97, 116, 32, 51, 0];
    }
}