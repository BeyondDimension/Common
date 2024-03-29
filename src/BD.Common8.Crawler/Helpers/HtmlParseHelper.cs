namespace BD.Common8.Crawler.Helpers;

/// <summary>
/// Html 助手类
/// </summary>
public static partial class HtmlParseHelper
{
    /// <summary>
    /// 解析表格
    /// </summary>
    /// <typeparam name="TTableRow">行解析结果的类型</typeparam>
    /// <param name="document">要解析的 HTML 内容</param>
    /// <param name="tableSelector">表格选择器</param>
    /// <param name="rowSelector">行选择器</param>
    /// <param name="rowParseFunc">行解析数据的方法</param>
    /// <returns>解析结果的异步可枚举集合</returns>
    public static async IAsyncEnumerable<TTableRow> ParseSimpleTable<TTableRow>(IDocument document,
        string tableSelector,
        string rowSelector,
        Func<IElement, ValueTask<TTableRow>> rowParseFunc)
    {
        var tableElement = document.QuerySelector(tableSelector);
        if (tableElement == null)
            yield break;

        var rowsElement = tableElement.QuerySelectorAll(rowSelector);
        if (rowsElement == null || rowsElement.Length == 0)
            yield break;

        foreach (var rowElement in rowsElement)
        {
            yield return await rowParseFunc(rowElement);
        }
    }

    /// <summary>
    /// 解析表格
    /// </summary>
    /// <typeparam name="TTableRow">行解析类型化结果</typeparam>
    /// <param name="tableElement">table 元素</param>
    /// <param name="includeThead">是否包含 thead 中的行</param>
    /// <param name="parseFunc">行解析方法</param>
    /// <returns></returns>
    public static async IAsyncEnumerable<TTableRow> ParseSimpleTable<TTableRow>(IElement? tableElement,
        bool includeThead,
        Func<IElement, ValueTask<TTableRow>> parseFunc)
    {
        if (tableElement == null)
            yield break;

        if (!tableElement.HasChildNodes)
            yield break;

        var rowsElement = includeThead
            ? tableElement.QuerySelectorAll("tr")
            : tableElement.QuerySelectorAll("tbody > tr");

        if (rowsElement == null || rowsElement.Length == 0)
            yield break;

        foreach (var rowElement in rowsElement)
        {
            yield return await parseFunc(rowElement);
        }
    }
}
