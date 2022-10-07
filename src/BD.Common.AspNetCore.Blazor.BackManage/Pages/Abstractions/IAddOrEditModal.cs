namespace BD.Common.Pages.Abstractions;

/// <summary>
/// 添加或编辑对话框
/// </summary>
/// <typeparam name="TEditDTO"></typeparam>
/// <typeparam name="TPrimaryKey"></typeparam>
public interface IAddOrEditModal<TEditDTO, TPrimaryKey> : ITableQuery where TEditDTO : class where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    string AddOrEditModalTitle { get; set; }

    bool AddOrEditModalVisible { get; set; }

    bool AddOrEditModalConfirmLoading { get; set; }

    TEditDTO AddOrEditModalEditDTO { get; set; }

    TPrimaryKey AddOrEditModalEditId { get; set; }
}
