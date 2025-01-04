using System.Runtime.Serialization;

namespace Flour.Commons.Models;

[DataContract]
public class PagedResult<T> : IResult<IEnumerable<T>>
{
    [DataMember(Order = 1)] public int Count { get; set; }
    [DataMember(Order = 2)] public int Page { get; set; }
    [DataMember(Order = 3)] public int Total { get; set; }
    [DataMember(Order = 4)] public IEnumerable<T> Data { get; set; }
    [DataMember(Order = 5)] public bool Success { get; set; }
    [DataMember(Order = 6)] public string Message { get; set; }
    [DataMember(Order = 7)] public int StatusCode { get; set; }
    [DataMember(Order = 8)] public int ErrorCode { get; set; }

    public static PagedResult<T> Ok(IEnumerable<T> data, int count = 0, int page = 0, int total = 0)
    {
        return New(null, 200, data, true, count, page, total);
    }

    public static PagedResult<T> Created(IEnumerable<T> data, int count = 0, int page = 0, int total = 0)
    {
        return New(null, 201, data, true, count, page, total);
    }

    public static PagedResult<T> Updated(IEnumerable<T> data = default, int count = 0, int page = 0, int total = 0)
    {
        return New(null, 204, data, true, count, page, total);
    }

    public static PagedResult<T> Bad(string message)
    {
        return New(message, 400);
    }

    public static PagedResult<T> Forbidden(string message)
    {
        return New(message, 401);
    }

    public static PagedResult<T> UnAuthorized(string message)
    {
        return New(message, 403);
    }

    public static PagedResult<T> NotFound(string message)
    {
        return New(message, 404);
    }

    public static PagedResult<T> Failed(string message, int code = 500)
    {
        return New(message, code);
    }

    public static PagedResult<T> Failed(IResult result)
    {
        return New(result.Message, result.StatusCode);
    }

    public static PagedResult<T> Internal(string message)
    {
        return New(message);
    }


    private static PagedResult<T> New(
        string message,
        int code = 500,
        IEnumerable<T> data = default,
        bool success = false,
        int count = 0,
        int page = 0,
        int total = 0)
    {
        return new()
        {
            Message = message,
            StatusCode = code,
            Success = success,
            Data = data,
            Count = count,
            Page = page,
            Total = total,
        };
    }
}