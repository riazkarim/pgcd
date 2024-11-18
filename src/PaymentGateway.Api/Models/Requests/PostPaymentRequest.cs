using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest : IValidatableObject
{
    [Required]
    [Length(14, 19)]
    [RegularExpression(@"^[0-9]$", ErrorMessage = "Card number should be numeric")]
    public string CardNumber { get; set; }
    [Required]
    [Range(1, 12)]
    public int ExpiryMonth { get; set; }
    [Required]
    public int ExpiryYear { get; set; }
    [Required]
    [RegularExpression(@"^(USD|GBP|EUR)$", ErrorMessage = "Invalid currency should be one of USD/GBP/EUR")]
    public string Currency { get; set; }
    [Required]
    public int Amount { get; set; }
    [Required]
    [Length(3, 4)]
    [RegularExpression(@"^[0-9]$", ErrorMessage = "Cvv should be numeric")]
    public string Cvv { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var date = new DateTime(ExpiryYear, ExpiryMonth, DateTime.DaysInMonth(ExpiryYear, ExpiryMonth));
        if (date < DateTime.UtcNow.Date) // Possible consideration required for time zones
        {
            yield return new ValidationResult("The expiry date is in the past");
        }
    }
}