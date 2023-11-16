namespace BD.Common8.Crawler.Helpers;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// Html 助手类
/// </summary>
public static partial class HtmlParseHelper
{
    public static async IAsyncEnumerable<TTableRow> ParseSimpleTable<TTableRow>(Stream stream, string tableSelector, string rowSelector, Func<IElement, ValueTask<TTableRow>> rowParseFunc)
    {
        IBrowsingContext browsingContext = BrowsingContext.New();

        var htmlParser = browsingContext.GetService<IHtmlParser>();

        if (htmlParser == null)
            throw new ArgumentException(nameof(htmlParser));

        IDocument document = await htmlParser.ParseDocumentAsync(stream);

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
    /// 解析建议表格
    /// </summary>
    /// <typeparam name="TTableRow">行解析类型化结果</typeparam>
    /// <param name="tableElement">table 元素</param>
    /// <param name="includeThead">是否包含 thead 中的行</param>
    /// <param name="parseFunc">行解析方法</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static async IAsyncEnumerable<TTableRow> ParseSimpleTable<TTableRow>(IElement? tableElement, bool includeThead, Func<IElement, ValueTask<TTableRow>> parseFunc)
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
