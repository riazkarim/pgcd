using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

/// <summary>
/// An interface to the acquiring bank service
/// </summary>
public interface IAcquiringBankService
{
    Task<AcquiringBankResponse> PostPaymentAsync(AcquiringBankRequest request);
}