using PaymentGateway.Api.Data;
using PaymentGateway.Api.Models.Data;

namespace PaymentGateway.Api.Services;

/// <summary>
/// Represents the repository where we store our payments.
/// </summary>
public interface IPaymentsRepository
{
    Task<Persisted<Guid, PaymentDetail>> AddAsync(PaymentDetail pd);
    Task<Persisted<Guid, PaymentDetail>> GetAsync(Guid id);
}