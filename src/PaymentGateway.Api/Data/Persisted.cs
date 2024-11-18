namespace PaymentGateway.Api.Models.Data;

public record PersistedPaymentDetail : PaymentDetail
{
    public Guid Id { get; init; }
}