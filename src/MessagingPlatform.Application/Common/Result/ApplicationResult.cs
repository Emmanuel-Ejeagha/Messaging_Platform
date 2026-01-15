using System;

namespace MessagingPlatform.Application.Common.Result;

public class ApplicationResult
{
    public bool Succeeded { get; protected set; }
    public string[] Errors { get; protected set; }

    public static ApplicationResult Success() => new() { Succeeded = true };
    public static ApplicationResult Failure(params string[] errors) => new() { Succeeded = false, Errors = errors };
}

public class ApplicationResult<T> : ApplicationResult
{
    public T? Data { get; set; }

    public static ApplicationResult<T> Success(T data) => new() { Succeeded = true, Data = data };
    public static ApplicationResult<T> Failure(params string[] errors) => new() { Succeeded = false, Errors = errors };
}
