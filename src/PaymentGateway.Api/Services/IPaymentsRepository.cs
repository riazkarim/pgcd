using PaymentGateway.Api.Models.Data;

namespace PaymentGateway.Api.Services;

/// <summary>
/// Represents the repository where we store our payments.
/// </summary>
public interface IPaymentsRepository
{
    Task<PersistedPaymentDetail> AddAsync(PaymentDetail pd);
    Task<PersistedPaymentDetail> GetAsync(Guid id);
}