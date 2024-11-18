namespace PaymentGateway.Api.Models.Data;

public class PaymentDetails
{
    public string CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Cvv { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public Guid AuthorizationCode { get; set; }
}