using Microsoft.AspNetCore.Mvc;
using SalesService.App.Common;
using SalesService.App.Common.Errors;

namespace SalesService.API.Common;

public class ApiControllerBase<T>(ILogger<T> logger) : ControllerBase
{
    private readonly ILogger<T> Logger = logger;
    protected IActionResult Problem(IErrorResult error)
    {
        var (statusCode, title, detail) = error switch
        {
            NotFoundError e => (StatusCodes.Status404NotFound, "Resource not found.", e.Message),
            Forbidden e => (StatusCodes.Status403Forbidden, "Operation not allowed.", e.Message),
            InvalidDisputeOperation e => (StatusCodes.Status422UnprocessableEntity, "Invalid operation.", e.Message),
            InvalidSaleOperation e => (StatusCodes.Status422UnprocessableEntity, "Invalid operation.", e.Message),

            _ => (StatusCodes.Status500InternalServerError, "Erro Interno do Servidor", "Ocorreu um erro inesperado.")
        };

        return Problem(
            statusCode: statusCode,
            title: title,
            detail: detail);
    }

    protected IActionResult HandleResult<Type>(Result<Type> result, Func<Type, IActionResult> onSuccess)
    {
        if (result.IsFailure)
        {
            if (result.Error is null)
                Logger.LogWarning("Result is success but value is empty. Is command handler returning a Result<T>?");

            return Problem(result.Error!);
        }

        if (result.Value is null)
            Logger.LogWarning("Result is success but value is empty. Is command handler returning a Result<T>?");

        return onSuccess(result.Value!);
    }
}
