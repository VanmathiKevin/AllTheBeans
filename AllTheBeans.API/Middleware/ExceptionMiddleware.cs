using AllTheBeans.Application.Exceptions;
using AllTheBeans.Infrastructure.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text.Json;

namespace AllTheBeans.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode;
            string message;

            switch (exception)
            {
                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;
                case ValidationException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;
                case DataAccessException:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "A data access error occurred.";
                    break;
                case SecurityTokenException:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = _env.IsDevelopment() ? exception.Message : "Authentication failed.";
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = _env.IsDevelopment() ? exception.Message : "An unexpected error occurred.";
                    break;
            }

            _logger.LogError(exception, "Exception caught in middleware: {Message}", message);

            var response = new
            {
                statusCode = (int)statusCode,
                message
            };

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
