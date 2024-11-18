using System.Collections.Concurrent;

using PaymentGateway.Api.Data;
using PaymentGateway.Api.Models.Data;

namespace PaymentGateway.Api.Services;

/// <summary>
/// A local implementation using a concurrent dictionary to store payments.
/// </summary>
public class PaymentsRepository : IPaymentsRepository
{
    private readonly ConcurrentDictionary<Guid, Persisted<Guid, PaymentDetails>> _payments = new();
    
    public async Task<Persisted<Guid, PaymentDetails>> AddAsync(PaymentDetails payment)
    {
        var pd = new Persisted<Guid, PaymentDetails>(Guid.NewGuid(), payment);
        _payments[pd.Id] = pd;
        return pd;
    }

    public async Task<Persisted<Guid, PaymentDetails>> GetAsync(Guid id)
    {
        await Task.FromResult(_payments.TryGetValue(id, out var pd));
        return pd;
    }
}