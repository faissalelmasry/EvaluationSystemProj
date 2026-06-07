using EvaluationSystem.Application.Exceptions;
using EvaluationSystem.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EvaluationSystem.Api.MiddleWare
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong during the request.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode = StatusCodes.Status500InternalServerError;
            string message = "Internal Server Error";

            if (exception is BadRequestException)
            {
                statusCode = StatusCodes.Status400BadRequest; 
                message = exception.Message;
            }
            else if (exception is UnauthorizedException)
            {
                statusCode = StatusCodes.Status401Unauthorized; 
                message = exception.Message;
            }else if(exception is NotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                message = exception.Message;
            }

            context.Response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(new { Message = message });

            return context.Response.WriteAsync(result);
        }
    }
}
