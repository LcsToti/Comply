using Microsoft.AspNetCore.Mvc;
using Payments.App.Common;
using Payments.App.Common.Errors;

namespace Payments.API.Common;

public class ApiControllerBase<T> : ControllerBase
{
    private readonly ILogger<T> _logger;
    public ApiControllerBase(ILogger<T> logger)
    {
        _logger = logger;
    }
    
    protected IActionResult Problem(IErrorResult error)
    {
        var (statusCode, title, detail) = error switch
        {
            NotFoundError e => (StatusCodes.Status404NotFound, "Resource not found.", e.Message),
            Conflict e => (StatusCodes.Status409Conflict, "Resource conflict.", e.Message),
            Forbidden e => (StatusCodes.Status403Forbidden, "Operation not allowed.", e.Message),
            InvalidPaymentOperation e => (StatusCodes.Status422UnprocessableEntity, "Invalid operation.", e.Message),
            InvalidRefundOperation e => (StatusCodes.Status422UnprocessableEntity, "Invalid operation.", e.Message),

            _ => (StatusCodes.Status500InternalServerError, "Erro Interno do Servidor", "Ocorreu um erro inesperado.")
        };

        return Problem(
            statusCode: statusCode,
            title: title,
            detail: detail);
    }
    
    protected IActionResult HandleResult<TResultValue>(Result<TResultValue> result, Func<TResultValue, IActionResult> onSuccess)
    {
        if (result.IsFailure)
        {
            if (result.Error is null)
                _logger.LogWarning("Result is success but value is empty. Is command handler returning a Result<T>?");

            return Problem(result.Error!);
        }

        if (result.Value is null)
            _logger.LogWarning("Result is success but value is empty. Is command handler returning a Result<T>?");

        return onSuccess(result.Value!);
    }
}