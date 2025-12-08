using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Payments.App.Common.Contracts;
using Payments.Domain.Aggregates.PaymentAccountAggregate;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Aggregates.PaymentAggregate.Factories;
using Payments.Infra.PaymentGateway.Exceptions;
using Payments.Infra.PaymentGateway.Mappers;
using Stripe;
using Refund = Payments.Domain.Aggregates.PaymentAggregate.Entities.Refund;
using PaymentMethod = Payments.Domain.Aggregates.PaymentAggregate.Entities.PaymentMethod;
using StripePaymentMethod = Stripe.PaymentMethod;

namespace Payments.Infra.PaymentGateway
{
    public class StripePaymentGateway : IPaymentGateway
    {
        private readonly ILogger<StripePaymentGateway> _logger;
        private readonly string? _apiKey;
        private readonly string? _refreshUrl;
        private readonly string? _returnUrl;

        public StripePaymentGateway(IConfiguration configuration, ILogger<StripePaymentGateway> logger)
        {
            _logger = logger;
            _apiKey = configuration["StripeConfiguration:ApiKey"];
            _refreshUrl = configuration["StripeConfiguration:RefreshUrl"];
            _returnUrl = configuration["StripeConfiguration:ReturnUrl"];

            if (!string.IsNullOrEmpty(_apiKey) || !string.IsNullOrEmpty(_refreshUrl) || !string.IsNullOrEmpty(_returnUrl)) return;

            _logger.LogError("Stripe API Key not found in settings.");
            throw new PaymentGatewayConnectionException("Stripe API Key not configured.");
        }

        #region Payments

        public async Task<Payment> CreatePaymentIntentAsync(long amount, string customerId, string paymentMethodId,
            CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new RequestOptions { ApiKey = _apiKey };
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Customer = customerId,
                    Currency = "brl",
                    PaymentMethod = paymentMethodId,
                    Confirm = true,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        AllowRedirects = "never",
                        Enabled = true
                    }
                };
                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options, requestOptions, cancellationToken);
                return StripeToDomainMapper.StripeToDomain(paymentIntent);
            }
            catch (StripeException ex)
            {
                _logger.LogError("Stripe API error creating Payment Intent;");
                throw new PaymentProcessingFailedException("Error creating Payment Intent.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error creating Payment Intent in Stripe.");
                throw new Exception("Unexpected error creating Payment Intent.", ex);
            }
        }

        public async Task<Refund> RefundPaymentIntentAsync(string paymentId, long amount, string reason,
            CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new RequestOptions { ApiKey = _apiKey };
                var options = new RefundCreateOptions { PaymentIntent = paymentId, Reason = "requested_by_customer", Amount = amount };
                var service = new RefundService();
                var stripeRefund = await service.CreateAsync(options, requestOptions, cancellationToken);

                return StripeToDomainMapper.StripeToDomain(stripeRefund);
            }
            catch (StripeException ex)
            {
                _logger.LogError("Stripe API error when refunding payment ({PaymentId})", paymentId);
                throw new PaymentProcessingFailedException("Error when refunding payment.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error when reversing payment ({PaymentId}) on Stripe.", paymentId);
                throw new Exception("Unexpected error when reversing payment.", ex);
            }
        }

        #endregion

        #region Customer

        public async Task<string> CreateCustomerAsync(string email, string name, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new RequestOptions { ApiKey = _apiKey };
                var options = new CustomerCreateOptions
                {
                    Email = email,
                    Name = name
                };
                var service = new CustomerService();
                var customer = await service.CreateAsync(options, requestOptions, cancellationToken);

                return customer.Id;
            }
            catch (StripeException ex)
            {
                _logger.LogError("An error occurred when Stripe tried to create a customer");
                throw new PaymentGatewayInvalidException("Error creating customer.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred when Stripe tried to create a customer");
                throw new Exception("Unexpected error creating customer.", ex);
            }
        }

        #endregion

        #region ConnectedAccount

        public async Task<string> CreateConnectedAccountAsync(string email, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new RequestOptions { ApiKey = _apiKey };
                var options = new AccountCreateOptions
                {
                    Type = "express",
                    Country = "BR",
                    Email = email,
                    Capabilities = new AccountCapabilitiesOptions
                    {
                        CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                        Transfers = new AccountCapabilitiesTransfersOptions { Requested = true }
                    }
                };
                var service = new AccountService();
                var account = await service.CreateAsync(options, requestOptions, cancellationToken);

                return account.Id;

            }
            catch (StripeException ex)
            {
                _logger.LogError("An error occurred when Stripe tried to create a connectedAccount");
                throw new PaymentGatewayInvalidException("Error creating connected account.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred when Stripe tried to create a connectedAccount");
                throw new Exception("Unexpected error creating connected account.", ex);
            }
        }

        public async Task<string> CreateAccountLink(string connectedAccountId, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new RequestOptions { ApiKey = _apiKey };
                var options = new AccountLinkCreateOptions
                {
                    Account = connectedAccountId,
                    RefreshUrl = _refreshUrl,
                    ReturnUrl = _returnUrl,
                    Type = "account_onboarding",
                };
                var service = new AccountLinkService();
                var accountLink = await service.CreateAsync(options, requestOptions, cancellationToken);

                return accountLink.Url;

            }
            catch (StripeException ex)
            {
                _logger.LogError("An error occurred when Stripe tried to create a account onboarding link");
                throw new PaymentGatewayInvalidException("Error creating account onboarding link.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred when Stripe tried to create a account onboarding link");
                throw new Exception("Unexpected error creating account onboarding link.", ex);
            }
        }

        public async Task<string> CreateDashboardLink(string connectedAccountId, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new RequestOptions { ApiKey = _apiKey };
                var service = new AccountLoginLinkService();
                var loginLink = await service.CreateAsync(connectedAccountId, requestOptions: requestOptions, cancellationToken: cancellationToken);

                return loginLink.Url;
            }
            catch (StripeException ex)
            {
                _logger.LogError("An error occurred when Stripe tried to create a dashboard link");
                throw new PaymentGatewayInvalidException("Error creating dashboard link.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred when Stripe tried to create a dashboard link");
                throw new Exception("Unexpected error creating dashboard link.", ex);
            }
        }

        public async Task TransferToConnectedAccountAsync(string connectedAccountId, string apiPaymentId, long amount,
            CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new RequestOptions { ApiKey = _apiKey };
                var options = new TransferCreateOptions
                {
                    Amount = amount,
                    Currency = "brl",
                    Destination = connectedAccountId,
                    SourceTransaction = apiPaymentId,

                };
                var service = new TransferService();
                await service.CreateAsync(options, requestOptions, cancellationToken: cancellationToken);

            }
            catch (StripeException ex)
            {
                _logger.LogError("An error occurred when Stripe tried to create a transfer");
                throw new PaymentProcessingFailedException("Error creating transfer.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred when Stripe tried to create a transfer");
                throw new Exception("Unexpected error creating transfer.", ex);
            }
        }

        public async Task<PaymentAccountStatus> GetConnectedAccountStatusAsync(
            string connectedAccountId,
            CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new RequestOptions { ApiKey = _apiKey };
                var service = new AccountService();
                Account account = await service.GetAsync(connectedAccountId, null, requestOptions, cancellationToken);

                if (account.PayoutsEnabled)
                    return PaymentAccountStatus.Active;

                if (account.Requirements.CurrentlyDue?.Any() == true)
                    return PaymentAccountStatus.Incomplete;

                if (account.Requirements.PendingVerification?.Any() == true)
                    return PaymentAccountStatus.PendingReview;

                if (account.Requirements.PastDue?.Any() == true)
                    return PaymentAccountStatus.Issues;

                return PaymentAccountStatus.None;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "An error occurred when Stripe tried get connected account status");
                throw new PaymentProcessingFailedException("Error getting connected account status.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred when Stripe tried to get connected account status");
                throw new Exception("Unexpected error getting connected account status.", ex);
            }
        }


    #endregion

        #region PaymentMethods
        public async Task<string> CreateSetupIntentAsync(string customerId, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new RequestOptions { ApiKey = _apiKey };
                var options = new SetupIntentCreateOptions
                {
                    Customer = customerId,
                    PaymentMethodTypes = new List<string> { "card" },
                };
                var service = new SetupIntentService();
                var setupIntent = await service.CreateAsync(options, requestOptions, cancellationToken);
                return setupIntent.ClientSecret;
            }
            catch (StripeException ex)
            {
                _logger.LogError("An error occurred when Stripe tried to create a setup intent");
                throw new PaymentGatewayInvalidException("Error creating setup intent.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred when Stripe tried to create a setup intent");
                throw new Exception("Unexpected error creating setup intent.", ex);
            }
        }

        public async Task<IReadOnlyCollection<PaymentMethod>> GetPaymentMethodsAsync(string customerId, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new RequestOptions { ApiKey = _apiKey };
                var service = new CustomerPaymentMethodService();
                var paymentMethods = await service.ListAsync(customerId, requestOptions: requestOptions,
                    cancellationToken: cancellationToken);

                return paymentMethods.Data
                    .Select(pm => PaymentMethodFactory.Create(
                        pm.Id,
                        pm.Type,
                        pm.Card?.Last4,
                        pm.Card?.Brand
                    ))
                    .ToList();
            }
            catch (StripeException ex)
            {
                _logger.LogError("An error occurred when Stripe tried to create a setup intent");
                throw new PaymentGatewayInvalidException("Error creating setup intent.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred when Stripe tried to create a setup intent");
                throw new Exception("Unexpected error creating setup intent.", ex);
            }
        }
        #endregion
    }
}