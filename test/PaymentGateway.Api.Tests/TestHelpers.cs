using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests;

public static class TestHelpers
{
    public static PaymentDetail ToPaymentDetails(this PostPaymentResponse response)
    {
        return new PaymentDetail()
        {
            Status = response.Status,
            Currency = response.Currency,
            Amount = response.Amount,
            CardNumberLastFour = response.CardNumberLastFour,
            ExpiryMonth = response.ExpiryMonth,
            ExpiryYear = response.ExpiryYear
        };
    }
}