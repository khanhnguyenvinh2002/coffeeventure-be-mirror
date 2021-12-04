using coffeeventureAPI.Core.API.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;


namespace coffeeventureAPI.Core.API
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            if (exception is NotFoundException)
            {
                code = HttpStatusCode.NotFound;
                WriteLogRecursive(exception);
            }
            else if (exception is BadRequestException)
            {
                code = HttpStatusCode.BadRequest;
                WriteLogRecursive(exception);
            }
            else if (exception is ForbiddenException)
            {
                code = HttpStatusCode.Forbidden;
                WriteLogRecursive(exception);
            }
            else if (exception is UnauthorizedException)
            {
                code = HttpStatusCode.Unauthorized;
            }
            else if (exception is ValidateErrorException)
            {
                code = HttpStatusCode.UnprocessableEntity;
            }
            else
            {
                WriteLogRecursive(exception);
            }

            var result = exception.Message;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }

        private void WriteLogRecursive(Exception ex)
        {
            _logger.LogError("{0}|{1}", ex.Source, ex.Message);
            if (ex.InnerException != null)
            {
                WriteLogRecursive(ex.InnerException);
            }
        }
    }
}
