namespace PaymentGateway.Api.Models.Data;

public record PaymentDetail
{
    public string CardNumberLastFour { get; init; }
    public int ExpiryMonth { get; init; }
    public int ExpiryYear { get; init; }
    public string Cvv { get; init; }
    public string Currency { get; init; }
    public int Amount { get; init; }
    public PaymentStatus Status { get; init; }
    public Guid AuthorizationCode { get; init; }
}