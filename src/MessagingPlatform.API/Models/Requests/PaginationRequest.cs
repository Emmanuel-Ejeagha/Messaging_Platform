using System;
using System.ComponentModel.DataAnnotations;

namespace MessagingPlatform.API.Models.Requests;

public class PaginationRequest
{
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}

public class MessagesPaginationRequest : PaginationRequest
{
    public bool IncludeThreada { get; set; } = false;
    public DateTime? BeforeDate { get; set; }
}
