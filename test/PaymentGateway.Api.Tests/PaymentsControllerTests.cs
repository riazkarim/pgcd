using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using NUnit.Framework;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Services.Exceptions;

namespace PaymentGateway.Api.Tests;

[TestFixture]
public class PaymentsControllerTests
{
    [Test]
    public async Task Should_RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = TestObjects.PostPaymentResponse;

        var paymentsRepository = new PaymentsRepository();
        var ppd = await paymentsRepository.AddAsync(payment.ToPaymentDetails());

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton<IPaymentsRepository>(paymentsRepository)))
            .CreateClient();

        // Act
        var response = await client.GetAsync($"/api/payments/{ppd.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(paymentResponse, Is.Not.Null);
    }

    [Test]
    public async Task Should_Return404_IfPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Should_ReturnRejectedStatus_IfInvalidModel()
    {
        var prMock = new Mock<IPaymentsRepository>();
        var abMock = new Mock<IAcquiringBankService>();
        
        HttpClient client = GetClient(prMock.Object, abMock.Object);

        // Act
        var response = await client.PostAsJsonAsync($"/api/payments", TestObjects.InvalidPostPaymentRequest_Expiry);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseBody = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        Assert.That(responseBody.Status, Is.EqualTo(PaymentStatus.Rejected));
    }

    [Test]
    public async Task Should_ReturnResponse_IfAuthorized()
    {
        var prMock = new Mock<IPaymentsRepository>();
        var abMock = new Mock<IAcquiringBankService>();
        abMock.Setup(a => a.PostPaymentAsync(It.IsAny<AcquiringBankRequest>())).ReturnsAsync(
            new AcquiringBankResponse() {Authorized = true, AuthorizationCode = Guid.NewGuid().ToString()});
        
        HttpClient client = GetClient(prMock.Object, abMock.Object);

        // Act
        var response = await client.PostAsJsonAsync($"/api/payments", TestObjects.PostPaymentRequest);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseBody = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        Assert.That(responseBody.Status, Is.EqualTo(PaymentStatus.Authorized));
    }
    
    [Test]
    public async Task Should_ReturnError_IfBankCallFails()
    {
        var prMock = new Mock<IPaymentsRepository>();
        var abMock = new Mock<IAcquiringBankService>();
        abMock.Setup(a => a.PostPaymentAsync(It.IsAny<AcquiringBankRequest>())).Throws<AcquiringBankUnavailableException>();
        
        HttpClient client = GetClient(prMock.Object, abMock.Object);

        // Act
        var response = await client.PostAsJsonAsync($"/api/payments", TestObjects.PostPaymentRequest);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    private static HttpClient GetClient(IPaymentsRepository pr, IAcquiringBankService abs)
    {
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services => ((ServiceCollection)services)
                    .AddSingleton<IPaymentsRepository>(pr)
                    .AddSingleton<IAcquiringBankService>(abs)))
            .CreateClient();
        return client;
    }
}