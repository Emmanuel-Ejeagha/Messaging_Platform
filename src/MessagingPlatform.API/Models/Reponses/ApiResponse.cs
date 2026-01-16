using System;

namespace MessagingPlatform.API.Models.Reponses;

public class ApiResponse
{
    public bool Success { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public ApiResponse(bool success = true)
    {
        Success = success;
    }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public ApiResponse(T data) : base(true)
    {
        Data = data;
    }

    public ApiResponse() : base(true) { }
}

public class PaginatedResponse<T> : ApiResponse<IEnumerable<T>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public long TotalCount { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    
    public PaginatedResponse(
        IEnumerable<T> data,
        int pageNumber,
        int pageSize,
        long totalCount) : base(data)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasPreviousPage = pageNumber > 1;
        HasNextPage = pageNumber < TotalPages;
    }
}