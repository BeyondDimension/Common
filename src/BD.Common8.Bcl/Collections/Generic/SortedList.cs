using Nito.Comparers;
using ComparerBuilder_ = Nito.Comparers.ComparerBuilder;

namespace System.Collections.Generic;

/// <summary>
/// 排序列表，实现 <see cref="IEnumerable"/> 和 <see cref="IEnumerable{T}"/>
/// </summary>
/// <typeparam name="T">列表元素的类型</typeparam>
public partial class SortedList<T> : IEnumerable, IEnumerable<T> where T : notnull
{
    /// <summary>
    /// 获取用于构建比较器的静态属性
    /// </summary>
    public static ComparerBuilderFor<T> ComparerBuilder => ComparerBuilder_.For<T>();

    SortedList<T, T> implement;

    /// <summary>
    /// 构造一个以指定的比较器进行排序的列表
    /// </summary>
    public SortedList(IComparer<T> comparer)
    {
        implement = new SortedList<T, T>(comparer);
    }

    /// <summary>
    /// 构造一个以指定的比较器进行排序，并包含指定集合中的元素的列表
    /// </summary>
    public SortedList(IComparer<T> comparer, IEnumerable<T> collection)
    {
        implement = new SortedList<T, T>(collection.ToDictionary(k => k, v => v), comparer);
    }

    /// <summary>
    /// 构造一个以指定的比较器进行排序，并具有指定初始容量的列表
    /// </summary>
    public SortedList(IComparer<T> comparer, int capacity)
    {
        implement = new SortedList<T, T>(capacity, comparer);
    }

    /// <summary>
    /// 获取列表中的元素数
    /// </summary>
    public int Count => implement.Count;

    /// <summary>
    /// 获取或设置用于排序元素的比较器
    /// </summary>
    public IComparer<T> Comparer
    {
        get => implement.Comparer;
        set
        {
            if (value != implement.Comparer)
            {
                implement = new SortedList<T, T>(implement, value);
            }
        }
    }

    /// <summary>
    /// 获取或设置列表的容量
    /// </summary>
    public int Capacity { get => implement.Capacity; set => implement.Capacity = value; }

    /// <summary>
    /// 向列表末尾添加元素
    /// </summary>
    /// <param name="item">要添加的元素</param>
    public void Add(T item) => implement.Add(item, item);

    /// <summary>
    /// 清空列表中的所有元素
    /// </summary>
    public void Clear() => implement.Clear();

    /// <summary>
    /// 判断列表是否包含指定元素
    /// </summary>
    /// <param name="item">要查找的元素</param>
    /// <returns>如果列表包含指定元素，返回 <see langword="true"/>；否则为 <see langword="false"/> </returns>
    public bool Contains(T item) => implement.ContainsKey(item);

    /// <summary>
    /// 获取指定元素在列表中第一次出现的索引
    /// </summary>
    /// <param name="item">要查找的元素</param>
    /// <returns>指定元素在列表中第一次出现的索引，如果未找到元素，则返回 <see langword="-1"/> </returns>
    public int IndexOf(T item) => implement.IndexOfKey(item);

    /// <summary>
    /// 从列表中移除指定元素的第一个匹配项
    /// </summary>
    /// <param name="item">要移除的元素</param>
    /// <returns>如果成功移除了元素，返回 <see langword="true"/>；否则为 <see langword="false"/> </returns>
    public bool Remove(T item) => implement.Remove(item);

    /// <summary>
    /// 移除列表中指定索引处的元素
    /// </summary>
    /// <param name="index">要移除的元素的索引</param>
    public void RemoveAt(int index) => implement.RemoveAt(index);

    /// <summary>
    /// 将排序列表转换为可枚举的序列
    /// </summary>
    public IEnumerable<T> AsEnumerable()
    {
        IEnumerable<T> result = this;
        return result;
    }

    /// <summary>
    /// 返回一个枚举器，以便循环访问列表中的元素
    /// </summary>
    public IEnumerator<T> GetEnumerator() => implement.Keys.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

partial class SortedList<T> : ICollection<T>, IReadOnlyCollection<T>
{
    /// <summary>
    /// 获取一个值，表示集合是否为只读
    /// </summary>
    bool ICollection<T>.IsReadOnly
    {
        get
        {
            ICollection<KeyValuePair<T, T>> collection = implement;
            return collection.IsReadOnly;
        }
    }

    /// <summary>
    /// 将集合的元素复制到指定的数组中，从特定的索引开始。
    /// </summary>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ICollection<T> collection = implement.Keys;
        collection.CopyTo(array, arrayIndex);
    }
}

partial class SortedList<T> : IList<T>, IReadOnlyList<T>
{
    /// <summary>
    /// 获取或设置指定索引处的元素
    /// </summary>
    /// <param name="index">要获取或设置的元素的索引</param>
    /// <returns>位于指定索引处的元素</returns>
    public T this[int index] { get => implement.Keys[index]; set => implement.Keys[index] = value; }

    /// <summary>
    /// 在列表中指定的索引位置插入元素
    /// </summary>
    /// <param name="index">要插入新元素的索引</param>
    /// <param name="item">要插入的元素</param>
    public void Insert(int index, T item) => implement.Keys.Insert(index, item);
}