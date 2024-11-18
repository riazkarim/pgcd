using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Helpers;

public static class ModelHelpers
{
    public static AcquiringBankRequest ToAcquiringBankRequest(this PostPaymentRequest request)
    {
        return new AcquiringBankRequest()
        {
            CardNumber = request.CardNumber,
            ExpiryDate = $"{request.ExpiryMonth:D2}/{request.ExpiryYear:D4}",
            Cvv = request.Cvv,
            Currency = request.Currency,
            Amount = request.Amount
        };
    }

    public static PaymentDetails ToPaymentDetails(this PostPaymentRequest request, PaymentStatus status,
        string authorizationCode)
    {
        return new PaymentDetails()
        {
            Status = status,
            AuthorizationCode = new Guid(authorizationCode),
            Currency = request.Currency,
            Amount = request.Amount,
            CardNumberLastFour = request.CardNumber.ToLastFour(),
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Cvv = request.Cvv
        };
    }
    
    public static PaymentDetails ToPaymentDetails(this PostPaymentResponse response)
    {
        return new PaymentDetails()
        {
            Status = response.Status,
            Currency = response.Currency,
            Amount = response.Amount,
            CardNumberLastFour = response.CardNumberLastFour,
            ExpiryMonth = response.ExpiryMonth,
            ExpiryYear = response.ExpiryYear
        };
    }

    public static PostPaymentResponse ToPostPaymentResponse(this PostPaymentRequest request, PaymentStatus status, Guid? id)
    {
        return new PostPaymentResponse()
        {
            CardNumberLastFour = request.CardNumber.ToLastFour(),
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency,
            Amount = request.Amount,
            Status = status,
            Id = id
        };
    }

    public static string ToLastFour(this string cardNumber)
    {
        return cardNumber.Substring(cardNumber.Length - 4, 4);
    }
}