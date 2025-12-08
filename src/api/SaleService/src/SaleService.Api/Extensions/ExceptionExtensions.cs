using SalesService.Domain.Aggregates.SaleAggregate.Exceptions;

namespace SalesService.API.Extensions;

public static class ExceptionExtensions
{
    public static (string Title, int StatusCode, string Message) ToProblemDetails(this Exception ex)
    {
      return ex switch
            {
                InvalidDeliveryException => ("Invalid delivery operation", 400, ex.Message),
                InvalidSaleException => ("Invalid sale operation", 400, ex.Message),
                InvalidDisputeException => ("Invalid dispute operation", 400, ex.Message),
                
                _ => ("Internal Server Error", 500, ex.Message)
            };
    }
}