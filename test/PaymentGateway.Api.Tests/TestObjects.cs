using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests;

public class TestObjects
{
    public static Random _random = new Random();

    public static PaymentDetail PaymentDetail = new PaymentDetail()
    {
        CardNumberLastFour = _random.Next(9999).ToString("D4"),
        AuthorizationCode = Guid.NewGuid(),
        ExpiryMonth = _random.Next(1, 12),
        ExpiryYear = DateTime.Today.AddYears(3).Year,
        Currency = "EUR",
        Amount = _random.Next(1000, 50000),
        Cvv = _random.Next(999).ToString("D3"),
        Status = PaymentStatus.Authorized
    };
    
    public static PostPaymentResponse PostPaymentResponse = new PostPaymentResponse
    {
        ExpiryYear = _random.Next(2023, 2030),
        ExpiryMonth = _random.Next(1, 12),
        Amount = _random.Next(1, 10000),
        CardNumberLastFour = _random.Next(1111, 9999).ToString(),
        Currency = "GBP"
    };

    public static PostPaymentRequest InvalidPostPaymentRequest = new PostPaymentRequest()
    {
        ExpiryYear = DateTime.Today.Year - 1, //Year older than today
        ExpiryMonth = _random.Next(1, 12),
        Amount = _random.Next(1, 10000),
        CardNumber = _random.Next(1111, 999999999).ToString(), //Invalid card number
        Currency = "GBP"
    };
}