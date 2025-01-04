namespace Flour.Commons.Models.PagedRequest;

public interface IPageContext
{
    int PageIndex { get; set; }
    int PageSize { get; set; }
    IEnumerable<SortDescriptor> ListSort { get; set; }
}

public interface IPageContext<T> : IPageContext
{
    T Filter { get; set; }
}