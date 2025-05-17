using AllTheBeans.Application.Exceptions;
using AllTheBeans.Infrastructure.Exceptions;
using System.Net;
using System.Text.Json;

namespace AllTheBeans.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
            string message = exception.Message;

            switch (exception)
            {
                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;
                case ValidationException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case DataAccessException:
                    statusCode = HttpStatusCode.InternalServerError;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
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
