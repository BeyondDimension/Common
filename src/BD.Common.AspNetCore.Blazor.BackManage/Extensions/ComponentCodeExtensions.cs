// ReSharper disable once CheckNamespace
namespace BD.Common;

public static partial class ComponentCodeExtensions
{
    /// <summary>
    /// 禁用或启用
    /// </summary>
    /// <param name="item"></param>
    /// <param name="api"></param>
    /// <param name="apiRelativeUrl"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<bool> ToggleStateCoreAsync(this IDisableLoading item, IApiConnection api, string apiRelativeUrl, CancellationToken cancellationToken = default)
    {
        item.DisableLoading = true;
        var rsp = await api.ApiDeleteAsync(apiRelativeUrl, cancellationToken);
        item.DisableLoading = false;
        if (rsp.IsSuccess)
        {
            item.Disable = !item.Disable;
        }
        return rsp.IsSuccess;
    }

    /// <summary>
    /// 设置第一条数据与最后一条数据的标识
    /// </summary>
    /// <param name="items"></param>
    static void SetFirstAndLast(this IReadOnlyList<IFirstAndLast> items)
    {
        var count = items.Count;
        if (count > 0)
        {
            items[0].IsFirst = true;
            items[count - 1].IsLast = true;
        }
    }

    //public static void SetFirstAndLast(this IReadOnlyPagedModel<IFirstAndLast> items)
    //{
    //    // TODO
    //}

    /// <summary>
    /// 上移或下移
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="thiz"></param>
    /// <param name="api"></param>
    /// <param name="apiRelativeUrl"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task MoveUpOrDownCoreAsync<T>(this T thiz, IApiConnection api, string apiRelativeUrl, CancellationToken cancellationToken = default) where T : IMoveUpOrDownLoading, ITableQuery
    {
        thiz.MoveUpOrDownLoading = true;
        var rsp = await api.ApiPutAsync(apiRelativeUrl, cancellationToken);
        if (rsp.IsSuccess)
        {
            await thiz.QueryAsync();
        }
        else
        {
            thiz.MoveUpOrDownLoading = false;
        }
    }

    /// <summary>
    /// 查询表格
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="thiz"></param>
    /// <param name="api"></param>
    /// <param name="apiRelativeUrl"></param>
    /// <param name="setQueryLoading"></param>
    /// <param name="handleData"></param>
    /// <returns></returns>
    public static async Task<bool> QueryCoreAsync<T>(this ITableQuery<T> thiz, IApiConnection api, string apiRelativeUrl, bool setQueryLoading = true, Action<T>? handleData = null)
    {
        bool isSuccess;
        if (setQueryLoading) thiz.QueryLoading = true;
        var rsp = await api.ApiGetAsync<T>(apiRelativeUrl);
        isSuccess = rsp.IsSuccess;
        if (isSuccess)
        {
            var data = rsp.Data!;
            handleData?.Invoke(data);
            if (data is IReadOnlyList<IFirstAndLast> data_fl)
            {
                data_fl.SetFirstAndLast();
            }
            thiz.DataSource = data;
            if (thiz is IMoveUpOrDownLoading imuodl)
            {
                imuodl.MoveUpOrDownLoading = false;
            }
        }
        thiz.QueryLoading = false;
        return isSuccess;
    }

    /// <inheritdoc cref="AddOrEditModalBtnOkOnClickCoreAsync{TAddOrEditDTO, TPrimaryKey, TAddDTO, TEditDTO}(IAddOrEditModal{TAddOrEditDTO, TPrimaryKey}, IApiConnection, MessageService?, string, Func{string, TAddDTO, Task{IApiResponse}}?, Func{string, TEditDTO, Task{IApiResponse}}?)"/>
    public static Task AddOrEditModalBtnOkOnClickCoreAsync<TEditDTO, TPrimaryKey>(this IAddOrEditModal<TEditDTO, TPrimaryKey> thiz, IApiConnection api, MessageService? message, string apiRelativeUrl,
        Func<string, TEditDTO, Task<IApiResponse>>? apiPostAsync = null,
        Func<string, TEditDTO, Task<IApiResponse>>? apiPutAsync = null)
        where TEditDTO : class
        where TPrimaryKey : notnull, IEquatable<TPrimaryKey> => AddOrEditModalBtnOkOnClickCoreAsync<TEditDTO, TPrimaryKey, TEditDTO, TEditDTO>(thiz, api, message, apiRelativeUrl, apiPostAsync, apiPutAsync);

    /// <summary>
    /// 添加或编辑对话框确认按钮点击事件
    /// </summary>
    /// <typeparam name="TAddOrEditDTO"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <typeparam name="TAddDTO"></typeparam>
    /// <typeparam name="TEditDTO"></typeparam>
    /// <param name="thiz"></param>
    /// <param name="api"></param>
    /// <param name="message"></param>
    /// <param name="apiRelativeUrl"></param>
    /// <param name="apiPostAsync"></param>
    /// <param name="apiPutAsync"></param>
    /// <returns></returns>
    public static async Task<IApiResponse> AddOrEditModalBtnOkOnClickCoreAsync<TAddOrEditDTO, TPrimaryKey, TAddDTO, TEditDTO>(
        this IAddOrEditModal<TAddOrEditDTO, TPrimaryKey> thiz,
        IApiConnection api,
        MessageService? message,
        string apiRelativeUrl,
        Func<string, TAddDTO, Task<IApiResponse>>? apiPostAsync = null,
        Func<string, TEditDTO, Task<IApiResponse>>? apiPutAsync = null)
        where TAddOrEditDTO : class
        where TAddDTO : class, TAddOrEditDTO
        where TEditDTO : class, TAddOrEditDTO
        where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
    {
        IApiResponse rsp;
        thiz.AddOrEditModalConfirmLoading = true;
        var isAdd = EqualityComparer<TPrimaryKey>.Default.Equals(thiz.AddOrEditModalEditId, default);
        if (isAdd)
        {
            var req = (TAddDTO)thiz.AddOrEditModalEditDTO;
            if (apiPostAsync == null)
            {
                rsp = await api.ApiPostAsync(apiRelativeUrl, req);
            }
            else
            {
                rsp = await apiPostAsync(apiRelativeUrl, req);
            }
        }
        else
        {
            if (thiz.AddOrEditModalEditDTO is not IKeyModel<TPrimaryKey>)
            {
                apiRelativeUrl = $"{apiRelativeUrl}/{thiz.AddOrEditModalEditId}";
            }
            var req = (TEditDTO)thiz.AddOrEditModalEditDTO;
            if (apiPutAsync == null)
            {
                rsp = await api.ApiPutAsync(apiRelativeUrl, req);
            }
            else
            {
                rsp = await apiPutAsync(apiRelativeUrl, req);
            }
        }
        if (rsp.IsSuccess)
        {
            if (message != null)
            {
                ShowSuccess();
                async void ShowSuccess() => await message.Success(isAdd ? SharedStrings.AddSuccess : SharedStrings.EditSuccess);
            }
            thiz.AddOrEditModalVisible = false;
            await thiz.QueryAsync();
        }
        thiz.AddOrEditModalConfirmLoading = false;
        return rsp;
    }

    /// <summary>
    /// 添加或编辑对话框取消按钮点击事件
    /// </summary>
    /// <typeparam name="TEditDTO"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="thiz"></param>
    public static void AddOrEditModalBtnCancelOnClickCore<TEditDTO, TPrimaryKey>(this IAddOrEditModal<TEditDTO, TPrimaryKey> thiz)
        where TEditDTO : class
        where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
    {
        thiz.AddOrEditModalVisible = false;
    }

    /// <summary>
    /// 添加或编辑
    /// </summary>
    /// <typeparam name="TEditDTO"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="thiz"></param>
    /// <param name="item"></param>
    /// <param name="title"></param>
    public static void AddOrEditCore<TEditDTO, TPrimaryKey>(this IAddOrEditModal<TEditDTO, TPrimaryKey> thiz, KeyModel<TPrimaryKey>? item, string? title = null)
        where TEditDTO : class
        where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
    {
        thiz.AddOrEditModalTitle = title ?? (item == null ? SharedStrings.Add : SharedStrings.Edit);
        thiz.AddOrEditModalEditId = item == null ? default! : item.Id;
        thiz.AddOrEditModalVisible = true;
    }

    public static void AddQueryStringDateRange(this IDateRange dateRange, IDictionary<string, string?> dict)
    {
        if (dateRange.DateRange != null)
        {
            if (dateRange.DateRange.Length >= 1)
            {
                var dateStart = dateRange.DateRange[0];
                if (dateStart.HasValue && dateStart.Value != default)
                {
                    dict.Add("dateStart", dateStart.Value.ToString("O"));
                }
                if (dateRange.DateRange.Length >= 2)
                {
                    var dateEnd = dateRange.DateRange[1];
                    if (dateEnd.HasValue && dateEnd.Value != default)
                    {
                        dict.Add("dateEnd", dateEnd.Value.ToString("O"));
                    }
                }
            }
        }
    }

    static class ConfirmIconRenderFragments
    {
        public static readonly RenderFragment Warning = (builder) =>
        {
            builder.OpenComponent<Icon>(0);
            builder.AddAttribute(1, "Type", "exclamation-circle");
            builder.AddAttribute(2, "Theme", "outline");
            builder.CloseComponent();
        };
    }

    /// <summary>
    /// 带确认框确认的删除操作
    /// </summary>
    /// <param name="item"></param>
    /// <param name="query"></param>
    /// <param name="api"></param>
    /// <param name="apiRelativeUrl"></param>
    /// <param name="modal"></param>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task DeleteCoreAsync(this IDeleteLoading item, ITableQuery query, IApiConnection api, string apiRelativeUrl, ModalService modal, MessageService? message, string content, CancellationToken cancellationToken = default)
    {
        var isOK = await modal.ConfirmAsync(new ConfirmOptions
        {
            Title = SharedStrings.Warning,
            Icon = ConfirmIconRenderFragments.Warning,
            Content = string.Format(SharedStrings.ConfirmContentDelete_, content),
            OkType = "danger",
            Width = 520D,
        });
        if (!isOK) return;

        item.DeleteLoading = true;
        var rsp = await api.ApiDeleteAsync(apiRelativeUrl, cancellationToken);
        if (rsp.IsSuccess)
        {
            if (message != null)
            {
                ShowSuccess();
                async void ShowSuccess() => await message.Success(SharedStrings.DeleteSuccess);
            }
            await query.QueryAsync();
        }
        else
        {
            item.DeleteLoading = false;
        }
    }

    /// <summary>
    /// 置于顶部或取消置顶
    /// </summary>
    /// <param name="item"></param>
    /// <param name="api"></param>
    /// <param name="apiRelativeUrl"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<bool> ToggleIsTopCoreAsync(this IIsTopLoading item, IApiConnection api, string apiRelativeUrl, CancellationToken cancellationToken = default)
    {
        item.IsTopLoading = true;
        var rsp = await api.ApiPutAsync(apiRelativeUrl, cancellationToken);
        item.IsTopLoading = false;
        if (rsp.IsSuccess)
        {
            item.IsTop = !item.IsTop;
        }
        return rsp.IsSuccess;
    }
}
