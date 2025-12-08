using ListingService.App.Common.Errors;
using ListingService.App.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace ListingService.Api.Common;

public class ApiControllerBase<T>(ILogger<T> logger) : ControllerBase
{
    private readonly ILogger<T> Logger = logger;
    protected IActionResult Problem(IError error)
    {
        var (statusCode, title, detail) = error switch
        {
            NotFound e => (StatusCodes.Status404NotFound, "Resource not found.", e.Message),
            Conflict e => (StatusCodes.Status409Conflict, "Resource conflict.", e.Message),
            Forbidden e => (StatusCodes.Status403Forbidden, "Operation not allowed.", e.Message),
            InvalidBidOperation e => (StatusCodes.Status422UnprocessableEntity, "Invalid operation.", e.Message),
            InvalidListingStatus e => (StatusCodes.Status422UnprocessableEntity, "Invalid operation.", e.Message),
            InvalidPurchaseOperation e => (StatusCodes.Status422UnprocessableEntity, "Invalid operation.", e.Message),

            _ => (StatusCodes.Status500InternalServerError, "Erro Interno do Servidor", "Ocorreu um erro inesperado.")
        };

        return Problem(
            statusCode: statusCode,
            title: title,
            detail: detail);
    }

    protected IActionResult HandleResult<Type>(Result<Type> result, Func<Type, IActionResult> onSucess)
    {
        if (result.IsFailure)
        {
            if (result.Error is null)
                Logger.LogWarning("Result is sucess but value is empty. Is command handler returning a Result<T>?");

            return Problem(result.Error!);
        }

        if (result.Value is null)
            Logger.LogWarning("Result is sucess but value is empty. Is command handler returning a Result<T>?");

        return onSucess(result.Value!);
    }
}
