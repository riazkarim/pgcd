using PaymentGateway.Api.Data;
using PaymentGateway.Api.Models.Data;

namespace PaymentGateway.Api.Services;

/// <summary>
/// Represents the repository where we store our payments.
/// </summary>
public interface IPaymentsRepository
{
    Task<Persisted<Guid, PaymentDetails>> AddAsync(PaymentDetails pd);
    Task<Persisted<Guid, PaymentDetails>> GetAsync(Guid id);
}