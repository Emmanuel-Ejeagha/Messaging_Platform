using System;
using FluentValidation.Results;

namespace MessagingPlatform.Application.Common.Exceptions;

public class ValidationException : ApplicationException
{
    public List<string> Errors { get; }

    public ValidationException(List<string> errors) :
        base("VALIDATION_ERROR", "One or more validation errors occurred")
    {
        Errors = errors;
    }
    
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this(failures.Select(f => f.ErrorMessage).ToList())
    {        
    }
}
