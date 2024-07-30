using DistributedSystem.Domain.Exceptions;
using System.Text.Json;
using DistributedSystem.Domain.Exceptions;

namespace DistributedSystem.API.Middleware;
internal sealed class ExceptionHandlingMiddleware : IMiddleware {

    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) 
        => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
		try {
            await next(context);
		} catch (Exception ex) {

			_logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
		}
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception) {
        var statusCode = GetStatusCode(exception);

        var response = new {
            title = GetTitle(exception),
            status = statusCode,
            detail = exception.Message,
            errors = GetErrors(exception),
        };

        httpContext.Response.ContentType = "application/json";

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static int GetStatusCode(Exception exception)
        => exception switch {

            ProductException.ProductFieldException => StatusCodes.Status406NotAcceptable, //should be remove later
            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            FluentValidation.ValidationException => StatusCodes.Status400BadRequest,
            FormatException => StatusCodes.Status422UnprocessableEntity,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetTitle(Exception exception)
        => exception switch {
            DomainException applicationException => applicationException.Title,
            _ => "Server Error"
        };

    private static IReadOnlyCollection<DistributedSystem.Application.Exceptions.ValidationError> GetErrors(Exception exception) {

        IReadOnlyCollection<DistributedSystem.Application.Exceptions.ValidationError> errors = null;

        if(exception is DistributedSystem.Application.Exceptions.ValidationException validationException)
            errors = validationException.Errors;

        return errors;
    }



}
