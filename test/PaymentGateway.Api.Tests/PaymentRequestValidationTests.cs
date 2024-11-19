using System.ComponentModel.DataAnnotations;

using NUnit.Framework;

namespace PaymentGateway.Api.Tests;

[TestFixture]
public class PaymentRequestValidationTests
{
    [Test]
    public void Should_FailValidation_IfExpiryDateInPast()
    {
        var model = TestObjects.InvalidPostPaymentRequest_Expiry;
        var ctx = new ValidationContext(model, null, null);
        Assert.That(() => Validator.ValidateObject(model, ctx), Throws.TypeOf<ValidationException>());
    }
    
    [Test]
    public void Should_FailValidation_IfShortCardNumber()
    {
        var model = TestObjects.InvalidPostPaymentRequest_CardNumber;
        var ctx = new ValidationContext(model, null, null);
        Assert.That(() => Validator.ValidateObject(model, ctx, true), Throws.TypeOf<ValidationException>());
    }
    
    [Test]
    public void Should_PassValidation_IfValid()
    {
        var model = TestObjects.PostPaymentRequest;
        var ctx = new ValidationContext(model, null, null);
        Validator.ValidateObject(model, ctx, true);
    }
}