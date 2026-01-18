namespace MessagingPlatform.Application.Common.Exceptions;

public abstract class ApplicationException : Exception
{
    protected ApplicationException(string title, string message)
        : base(message) => Title = title;

    public string Title { get; }
}

public class NotFoundException : ApplicationException
{
    public NotFoundException(string name, object key)
        : base("Not Found", $"Entity '{name}' ({key}) was not found.")
    {
    }
}

public class ValidationException : ApplicationException
{
    public ValidationException(string message)
        : base("Validation Error", message)
    {
    }

    public ValidationException(IEnumerable<string> errors)
        : base("Validation Error", string.Join("; ", errors))
    {
    }
}

public class ForbiddenException : ApplicationException
{
    public ForbiddenException(string message)
        : base("Forbidden", message)
    {
    }
}

public class ConflictException : ApplicationException
{
    public ConflictException(string message)
        : base("Conflict", message)
    {
    }
}