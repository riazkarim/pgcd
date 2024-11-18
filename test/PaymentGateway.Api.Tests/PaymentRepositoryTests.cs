using NUnit.Framework;

using PaymentGateway.Api.Services;

using Assert = NUnit.Framework.Assert;

namespace PaymentGateway.Api.Tests;

[TestFixture]
public class PaymentRepositoryTests
{
    private PaymentsRepository _paymentsRepository;

    [SetUp]
    public void SetUp()
    {
        _paymentsRepository = new PaymentsRepository();
    }
    
    [Test]
    public async Task Should_StoreAndRetrievePayment()
    {
        var pd = TestObjects.PaymentDetail;
        var id = await _paymentsRepository.AddAsync(pd);
        var repod = await _paymentsRepository.GetAsync(id.Id);
        
        Assert.That(pd, Is.EqualTo(repod));
    }
}