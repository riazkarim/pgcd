using System.Collections.Concurrent;

using PaymentGateway.Api.Models.Data;

namespace PaymentGateway.Api.Services;

/// <summary>
/// A local implementation using a concurrent dictionary to store payments.
/// </summary>
public class PaymentsRepository : IPaymentsRepository
{
    private readonly ConcurrentDictionary<Guid, PersistedPaymentDetail> _payments = new();
    
    public async Task<PersistedPaymentDetail> AddAsync(PaymentDetail payment)
    {
        var pd = new PersistedPaymentDetail
        {
            Id = Guid.NewGuid(), 
            Amount = payment.Amount,
            Currency = payment.Currency,
            Cvv = payment.Cvv,
            Status = payment.Status,
            AuthorizationCode = payment.AuthorizationCode,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            CardNumberLastFour = payment.CardNumberLastFour
        };
        _payments[pd.Id] = pd;
        return pd;
    }

    public async Task<PersistedPaymentDetail> GetAsync(Guid id)
    {
        await Task.FromResult(_payments.TryGetValue(id, out var pd));
        return pd;
    }
}