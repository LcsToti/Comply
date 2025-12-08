using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.API.Common;
using Payments.App.Common;
using Payments.App.Common.Results;
using Payments.App.UseCases.PaymentCases.GetLastSuccessfulPaymentsCount;
using Payments.App.UseCases.PaymentCases.GetPayment;
using Payments.App.UseCases.PayoutCases.WithdrawAllAvailablePayments;
using Payments.App.UseCases.PayoutCases.WithdrawPayment;

namespace Payments.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class PaymentsController : ApiControllerBase<PaymentsController>
    {
        private readonly IMediator _mediator;
        
        public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger) : base(logger)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet("LastSuccessful")]
        public async Task<IActionResult> GetLastSuccessfulPaymentsCount([FromQuery] int amount = 10)
        {
            var command = new GetLastSuccessfulPaymentsCountQuery(amount);
            
            var result = await _mediator.Send(command);
            
            return  Ok(result);
        }
        
        
        [HttpGet("{paymentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByPaymentId(Guid paymentId)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();
            
            var role = User.FindFirstValue(ClaimTypes.Role);
            
            var command = new GetPaymentCommand(paymentId, userId, role);
            
            Result<PaymentResult> result = await _mediator.Send(command);
            
            return HandleResult(result, value => Ok(value));
        }
        
        [HttpPost("withdraw/{paymentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> WithdrawPayment(Guid paymentId)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();
            
            var command = new WithdrawPaymentCommand(paymentId, userId);
            
            Result<PaymentResult> result = await _mediator.Send(command);
            
            return HandleResult(result, value => Ok(value));
        }

        [HttpPost("withdraw-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]       
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> WithdrawAllPayments()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();
            
            var command = new WithdrawAllAvailablePaymentsCommand(userId);
            
            Result<PaymentResult[]> result = await _mediator.Send(command);
            
            return HandleResult(result, value => Ok(value));
        }
    }
}

