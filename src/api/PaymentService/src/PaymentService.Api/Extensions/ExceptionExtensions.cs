using Payments.Domain.Exceptions;
using Payments.Domain.Exceptions.PaymentAccountExceptions;
using Payments.Domain.Exceptions.PaymentExceptions;
using Payments.Domain.Exceptions.PaymentMethodExceptions;
using Payments.Domain.Exceptions.RefundExceptions;
using Payments.Infra.PaymentGateway.Exceptions;

namespace Payments.API.Extensions;

public static class ExceptionExtensions
{
    public static (string Title, int StatusCode, string Message) ToProblemDetails(this Exception ex)
    {
      return ex switch
            {
                InvalidConnectedAccountDataException => ("Invalid Connected Account Data", 400, ex.Message),
                InvalidCustomerIdException => ("Invalid Customer ID", 400, ex.Message),
                InvalidPaymentAccountParamsException => ("Invalid Payment Account Parameters", 400, ex.Message),
                InvalidPaymentParamsException => ("Invalid Payment Parameters", 400, ex.Message),
                InvalidPaymentMethodParamsException => ("Invalid Payment Method Parameters", 400, ex.Message),
                InvalidRefundAmountException => ("Invalid Refund Amount", 400, ex.Message),
                InvalidRefundParamsException => ("Invalid Refund Parameters", 400, ex.Message),
                RequiredValueObjectException => ("Required Information Missing", 400, ex.Message),

                PaymentNotRefundableException => ("Payment Not Refundable", 409, ex.Message),
                PaymentRefundableAmountExceededException => ("Refund Amount Exceeded", 409, ex.Message),
                PaymentStatusChangeException => ("Invalid Payment Status Change", 409, ex.Message),
                PaymentWithdrawalException => ("Payment Withdrawal Error", 409, ex.Message),

                PaymentGatewayConnectionException => ("Payment Gateway Connection Error", 503, ex.Message),
                PaymentGatewayInvalidException => ("Payment Gateway Configuration Error", 500, ex.Message),
                PaymentProcessingFailedException => ("Payment Processing Failed", 502, ex.Message),

                _ => ("Internal Server Error", 500, ex.Message)
            };
    }
}