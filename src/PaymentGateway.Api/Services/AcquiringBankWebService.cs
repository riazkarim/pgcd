using Flurl.Http;

using PaymentGateway.Api.Configuration;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services.Exceptions;

namespace PaymentGateway.Api.Services;

/// <summary>
/// A wrapper to call an acquiring bank service using Http
/// </summary>
public class AcquiringBankWebService : IAcquiringBankService
{
    private readonly ILogger<AcquiringBankWebService> _logger;
    private readonly string _url;

    public AcquiringBankWebService(ILogger<AcquiringBankWebService> logger, PaymentGatewayConfiguration configuration)
    {
        _logger = logger;
        this._url = configuration.AcquiringBankUrl;
    }

    public async Task<AcquiringBankResponse> PostPaymentAsync(AcquiringBankRequest request)
    {
        try
        {
            var call = await _url.PostJsonAsync(request);
            var response = await call.GetJsonAsync<AcquiringBankResponse>();
            return response;
        }
        catch (FlurlHttpException ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new AcquiringBankUnavailableException();
        }
    }
}