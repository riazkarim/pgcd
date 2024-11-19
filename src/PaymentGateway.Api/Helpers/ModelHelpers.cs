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

    public static PaymentDetail ToPaymentDetails(this PostPaymentRequest request, PaymentStatus status,
        string? authorizationCode)
    {
        Guid.TryParse(authorizationCode, out var authCode);
        
        return new PaymentDetail()
        {
            Status = status,
            AuthorizationCode = authCode,
            Currency = request.Currency,
            Amount = request.Amount,
            CardNumberLastFour = request.CardNumber.ToLastFour(),
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Cvv = request.Cvv
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
        if (cardNumber.Length < 4)
        {
            return cardNumber;
        }
        else
        {
            return cardNumber.Substring(cardNumber.Length - 4, 4);
        }
    }
}