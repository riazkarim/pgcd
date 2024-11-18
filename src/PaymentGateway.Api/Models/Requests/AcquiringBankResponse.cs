using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Requests;

public record AcquiringBankResponse
{
    [JsonPropertyName("authorized")] public required bool Authorized { get; init; }

    [JsonPropertyName("authorization_code")]
    public required string AuthorizationCode { get; init; }
}