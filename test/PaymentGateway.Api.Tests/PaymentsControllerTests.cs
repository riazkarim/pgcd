using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using NUnit.Framework;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Helpers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

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
        var response = await client.GetAsync($"/api/Payments/{ppd.Id}");
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
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Should_ReturnRejectedStatus_IfInvalidModel()
    {
        var abMock = new Mock<IAcquiringBankService>();
        
        HttpClient client = GetClient(abMock.Object);

        // Act
        var response = await client.PostAsJsonAsync($"/api/payments", TestObjects.InvalidPostPaymentRequest);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseBody = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        Assert.That(responseBody.Status, Is.EqualTo(PaymentStatus.Rejected));
    }

    [Test]
    public async Task Should_ReturnResponse_IfAuthorized()
    {
        var abMock = new Mock<IAcquiringBankService>();
        abMock.Setup(a => a.PostPaymentAsync(It.IsAny<AcquiringBankRequest>())).ReturnsAsync(
            new AcquiringBankResponse() {Authorized = true, AuthorizationCode = Guid.NewGuid().ToString()});
        
        HttpClient client = GetClient(abMock.Object);

        // Act
        var response = await client.PostAsJsonAsync($"/api/payments", TestObjects.PostPaymentRequest);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseBody = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        Assert.That(responseBody.Status, Is.EqualTo(PaymentStatus.Authorized));
    }

    private static HttpClient GetClient(IAcquiringBankService abMock)
    {
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services => ((ServiceCollection)services)
                    .AddSingleton<IPaymentsRepository>(new PaymentsRepository())
                    .AddSingleton<IAcquiringBankService>(abMock)))
            .CreateClient();
        return client;
    }
}