using ListingService.App.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ListingService.Infra.Services;

public class HttpProductServiceClient(HttpClient http, ILogger<HttpProductServiceClient> logger) : IProductServiceClient
{
    private readonly HttpClient _http = http ?? throw new ArgumentNullException(nameof(http));
    private readonly ILogger<HttpProductServiceClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Guid?> GetSellerIdByProductIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting sellerId of product with ID: {ProductId}", productId);

        try
        {
            var response = await _http.GetAsync($"Products/{productId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Product {ProductId} not found.", productId);
                    return null;
                }

                _logger.LogError("Unexpected response {StatusCode} while checking product {ProductId}.", response.StatusCode, productId);
                return null;
            }

            // Desserializa e pega o sellerId
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            if (doc.RootElement.TryGetProperty("sellerId", out var sellerIdProp) &&
                Guid.TryParse(sellerIdProp.GetString(), out var sellerId))
            {
                return sellerId;
            }

            _logger.LogError("sellerId not found in response for product {ProductId}", productId);
            return null;
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("Timeout while checking product {ProductId}.", productId);
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while checking product {ProductId}.", productId);
            return null;
        }
    }
}