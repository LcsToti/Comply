using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.API.Common;
using Payments.App.Common;
using Payments.App.Common.Results;
using Payments.App.UseCases.UserCases.CreatePaymentMethods;
using Payments.App.UseCases.UserCases.GetConnectedAccountStatus;
using Payments.App.UseCases.UserCases.GetTotalWithdrawableAmount;
using Payments.App.UseCases.UserCases.GetUserDashboardLink;
using Payments.App.UseCases.UserCases.GetUserOnboardingLink;
using Payments.App.UseCases.UserCases.GetUserPaymentMethods;
using Payments.Domain.Aggregates.PaymentAccountAggregate;

namespace Payments.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class PaymentAccountsController : ApiControllerBase<PaymentAccountsController>
    {
        private readonly IMediator _mediator;
        
        public PaymentAccountsController(IMediator mediator, ILogger<PaymentAccountsController> logger) : base(logger)
        {
            _mediator = mediator;
        }
        
        [HttpGet("onboarding-link")]
        [ProducesResponseType<string>(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        public async Task<IActionResult> GetOnboardingLink()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();
            
            var command = new GetUserOnboardingLinkCommand(userId);
            
            Result<string> result = await _mediator.Send(command);

            return HandleResult(result, value => Ok(value));
        }
        
        [HttpGet("dashboard-link")]
        [ProducesResponseType<string>(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        public async Task<IActionResult> GetDashboardLink()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();
            
            var command = new GetUserDashboardLinkCommand(userId);
            
            Result<string> result = await _mediator.Send(command);

            return HandleResult(result, value => Ok(value));
        }
        
        [HttpGet("cash-balance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserBalance()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();
            
            var command = new GetTotalWithdrawableAmountCommand(userId);
            Result<decimal> result = await _mediator.Send(command);
            
            return HandleResult(result, value => Ok(value));
        }
        
        [HttpGet("payment-methods")]
        [ProducesResponseType<IReadOnlyCollection<PaymentMethodResult>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPaymentMethods()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();
            
            var command = new GetUserPaymentMethodsCommand(userId);
            Result<IReadOnlyCollection<PaymentMethodResult>> result = await _mediator.Send(command);
            
            return HandleResult(result, value => Ok(value));
        }
        
        [HttpGet("status")]
        [ProducesResponseType<bool>(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetConnectedAccountStatus()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();
            
            var command = new GetConnectedAccountStatusCommand(userId);
            
            Result<PaymentAccountStatus> result = await _mediator.Send(command);

            return HandleResult(result, value => Ok(value));
        }
        
        [HttpPost("payment-methods")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreatePaymentMethod()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();
            
            var command = new CreatePaymentMethodsCommand(userId);
            Result<string> result = await _mediator.Send(command);
            
            return HandleResult(result, value => Ok(value));
        }
    }
}

