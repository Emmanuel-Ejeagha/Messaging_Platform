using MediatR;
using MessagingPlatform.API.Models.Reponses;
using MessagingPlatform.Application.Common.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessagingPlatform.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected readonly IMediator _mediator;
        protected readonly ILogger _logger;

        public ApiControllerBase(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        protected IActionResult Success<T>(T data)
        {
            return Ok(new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Timestamp = DateTime.UtcNow
            });
        }

        protected IActionResult Success()
        {
            return Ok(new ApiResponse
            {
                Success = true,
                Timestamp = DateTime.UtcNow
            });
        }

        protected IActionResult Error(string message, int statusCode = 400)
        {
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "An error occurred",
                Detail = message,
                Instance = HttpContext.TraceIdentifier
            };

            return new ObjectResult(problemDetails)
            {
                StatusCode = statusCode
            };
        }

        protected IActionResult FromApplicationResult(ApplicationResult result)
        {
            if (result.Succeeded)
                return Success();

            return Error(string.Join(", ", result.Errors));
        }
        
        protected IActionResult FromApplicationResult<T>(ApplicationResult<T> result)
        {
            if (result.Succeeded && result.Data != null)
                return Success(result.Data);

            if (result.Succeeded)
                return Success();

            return Error(string.Join(", ", result.Errors));
        }
    }
}
