using System.Runtime.Serialization;

namespace Flour.Commons.Models.PagedRequest;

[DataContract]
public class PageContext : IPageContext
{
    public PageContext()
    {
        ListSort = Array.Empty<SortDescriptor>();
    }

    public PageContext(
        int pageIndex,
        int pageSize,
        IEnumerable<SortDescriptor> listSort = null)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        ListSort = listSort ?? Array.Empty<SortDescriptor>();
    }

    [DataMember(Order = 1)] public int PageIndex { get; set; }
    [DataMember(Order = 2)] public int PageSize { get; set; }
    [DataMember(Order = 3)] public IEnumerable<SortDescriptor> ListSort { get; set; }

    public (bool, string) GetSortingField()
    {
        var sort = ListSort?.FirstOrDefault();
        return sort is null ? (true, null) : (sort.Order == EnumSortDirection.Desc, sort.Field?.ToLower());
    }
}

[DataContract]
public class PageContext<T> : IPageContext<T>
    where T : class, new()
{
    public PageContext()
    {
        ListSort = Array.Empty<SortDescriptor>();
        Filter = new T();
    }

    public PageContext(
        int pageIndex,
        int pageSize,
        IEnumerable<SortDescriptor> listSort = null,
        T filter = null)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        ListSort = listSort ?? Array.Empty<SortDescriptor>();
        Filter = filter ?? new T();
    }

    [DataMember(Order = 1)] public int PageIndex { get; set; }
    [DataMember(Order = 2)] public int PageSize { get; set; }
    [DataMember(Order = 3)] public IEnumerable<SortDescriptor> ListSort { get; set; }
    [DataMember(Order = 4)] public T Filter { get; set; }

    public (bool desc, string field) GetSortingField()
    {
        var sort = ListSort?.FirstOrDefault();
        return sort is null ? (true, null) : (sort.Order == EnumSortDirection.Desc, sort.Field?.ToLower());
    }

    public bool IsValid()
    {
        return PageIndex > 0 && PageSize > 0
                             && Filter != null && ListSort != null;
    }
}