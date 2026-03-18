using System.Net;
using Microsoft.Extensions.Logging;
using OrderHub.Domain.Exceptions;

namespace OrderHub.Adapters.Inbound.Api.Middleware;

/// <summary>
/// Middleware para tratamento centralizado de exceções e logging de erros
/// Captura exceções não tratadas e registra com contexto completo
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            _logger.LogError(
                ex,
                "Erro não tratado na requisição {Method} {Path} | IP: {IpAddress} | UserAgent: {UserAgent}",
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress,
                context.Request.Headers.UserAgent);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case DomainException domainEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = domainEx.Message;
                response.ErrorCode = "DOMAIN_ERROR";
                break;

            case ArgumentException argEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = argEx.Message;
                response.ErrorCode = "ARGUMENT_ERROR";
                break;

            case KeyNotFoundException notFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = notFoundEx.Message;
                response.ErrorCode = "NOT_FOUND";
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "Erro interno do servidor. Por favor, tente novamente mais tarde.";
                response.ErrorCode = "INTERNAL_SERVER_ERROR";
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }

    private class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
