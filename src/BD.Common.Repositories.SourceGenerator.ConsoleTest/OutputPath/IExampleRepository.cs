#nullable enable
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由包 BD.Common.Repositories.SourceGenerator 源生成。
//     运行时版本：4.8.9139.0
//     编译器版本：4.6.0-3.23259.8 (c3cc1d0c)
//     生成器版本：1.23.10613.11400 (c239958a)
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------
// ReSharper disable once CheckNamespace
namespace BD.Common.Repositories.SourceGenerator.ConsoleTest.Repositories.Abstractions;

/// <summary>
/// 示例 - 仓储层接口
/// </summary>
public partial interface IExampleRepository : IRepository<Example, Guid>, IEFRepository
{
    /// <summary>
    /// 分页查询示例表格
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="disable">是否禁用</param>
    /// <param name="fileName">文件名</param>
    /// <param name="url">访问地址</param>
    /// <param name="createUser">创建人（创建此条目的后台管理员）</param>
    /// <param name="creationTime">创建时间</param>
    /// <param name="operatorUser">最后一次操作的人（记录后台管理员禁用或启用或编辑该条的操作）</param>
    /// <param name="name">名称</param>
    /// <param name="email">邮箱</param>
    /// <param name="architecture">Architecture</param>
    /// <param name="gender">性别</param>
    /// <param name="nickName">昵称</param>
    /// <param name="current">当前页码，页码从 1 开始，默认值：<see cref="IPagedModel.DefaultCurrent"/></param>
    /// <param name="pageSize">页大小，如果为 0 必定返回空集合，默认值：<see cref="IPagedModel.DefaultPageSize"/></param>
    /// <returns>示例分页表格查询结果数据</returns>
    Task<PagedModel<TableExampleDTO>> QueryAsync(
        string? title,
        bool? disable,
        string? fileName,
        string? url,
        string? createUser,
        DateTimeOffset[]? creationTime,
        string? operatorUser,
        string? name,
        string? email,
        Architecture? architecture,
        Gender? gender,
        string? nickName,
        int current = IPagedModel.DefaultCurrent,
        int pageSize = IPagedModel.DefaultPageSize);

    /// <summary>
    /// 获取用于【选择框/下拉列表(Select)】的数据
    /// </summary>
    /// <param name="takeCount">要获取的最大数量限制，默认值：<see cref="SelectItemDTO.Count"/></param>
    /// <returns>用于【选择框/下拉列表(Select)】的数据</returns>
    Task<SelectItemDTO<Guid>[]> GetSelectAsync(int takeCount = SelectItemDTO.Count);

    /// <summary>
    /// 根据【提供查询功能的数据源】设置【是否禁用】
    /// </summary>
    /// <param name="query">提供查询功能的数据源</param>
    /// <param name="disable">是否禁用，当值为 <see langword="true"/> 时禁用，为 <see langword="false"/> 时启用</param>
    /// <returns>受影响的行数</returns>
    Task<int> SetDisableAsync(IQueryable<Example> query, bool disable);

    /// <summary>
    /// 根据【主键】设置设置【是否禁用】
    /// </summary>
    /// <param name="operatorUserId">最后一次操作的人（记录后台管理员禁用或启用或编辑该条的操作）</param>
    /// <param name="id">主键</param>
    /// <param name="disable">是否禁用，当值为 <see langword="true"/> 时禁用，为 <see langword="false"/> 时启用</param>
    /// <returns>受影响的行数</returns>
    Task<int> SetDisableByIdAsync(Guid? operatorUserId, Guid id, bool disable);

    /// <summary>
    /// 根据主键获取【编辑模型】
    /// </summary>
    /// <param name="id">主键</param>
    /// <returns>编辑模型</returns>
    Task<EditExampleDTO?> GetEditByIdAsync(Guid id);

    /// <summary>
    /// 根据【编辑模型】更新一条数据
    /// </summary>
    /// <param name="operatorUserId">最后一次操作的人（记录后台管理员禁用或启用或编辑该条的操作）</param>
    /// <param name="id">主键</param>
    /// <param name="model">编辑模型</param>
    /// <returns>受影响的行数</returns>
    Task<int> UpdateAsync(Guid? operatorUserId, Guid id, EditExampleDTO model);

    /// <summary>
    /// 根据【添加模型】新增一条数据
    /// </summary>
    /// <param name="model">添加模型</param>
    /// <returns>受影响的行数</returns>
    Task<int> InsertAsync(AddExampleDTO model);

}

