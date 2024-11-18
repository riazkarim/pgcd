using System.Collections.Concurrent;

using PaymentGateway.Api.Data;
using PaymentGateway.Api.Models.Data;

namespace PaymentGateway.Api.Services;

/// <summary>
/// A local implementation using a concurrent dictionary to store payments.
/// </summary>
public class PaymentsRepository : IPaymentsRepository
{
    private readonly ConcurrentDictionary<Guid, Persisted<Guid, PaymentDetail>> _payments = new();
    
    public async Task<Persisted<Guid, PaymentDetail>> AddAsync(PaymentDetail payment)
    {
        var pd = new Persisted<Guid, PaymentDetail>(Guid.NewGuid(), payment);
        _payments[pd.Id] = pd;
        return pd;
    }

    public async Task<Persisted<Guid, PaymentDetail>> GetAsync(Guid id)
    {
        await Task.FromResult(_payments.TryGetValue(id, out var pd));
        return pd;
    }
}