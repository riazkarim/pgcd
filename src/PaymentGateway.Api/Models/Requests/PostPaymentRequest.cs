using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Requests;

public record PostPaymentRequest : IValidatableObject
{
    [Required]
    [Length(14, 19)]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "Card number should be numeric")]
    public string CardNumber { get; init; }
    [Required]
    [Range(1, 12)]
    public int ExpiryMonth { get; init; }
    [Required]
    public int ExpiryYear { get; init; }
    [Required]
    [RegularExpression(@"^(USD|GBP|EUR)$", ErrorMessage = "Invalid currency should be one of USD/GBP/EUR")]
    public string Currency { get; init; }
    [Required]
    public int Amount { get; init; }
    [Required]
    [Length(3, 4)]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "Cvv should be numeric")]
    public string Cvv { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var date = new DateTime(ExpiryYear, ExpiryMonth, DateTime.DaysInMonth(ExpiryYear, ExpiryMonth));
        if (date < DateTime.UtcNow.Date) // Possible consideration required for time zones
        {
            yield return new ValidationResult("The expiry date is in the past");
        }
    }
}