using Flurl.Http;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Services.Exceptions;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly ILogger<PaymentsController> _logger;
    private readonly IAcquiringBankService _abService;

    public PaymentsController(IPaymentsRepository paymentsRepository, ILogger<PaymentsController> logger, IAcquiringBankService abService)
    {
        _paymentsRepository = paymentsRepository;
        _logger = logger;
        _abService = abService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = await _paymentsRepository.GetAsync(id);

        if (payment == null)
        {
            return new NotFoundObjectResult(payment);
        }
        else
        {
            return new OkObjectResult(payment);
        }
    }

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse?>> PostPaymentAsync(PostPaymentRequest request)
    {
        //Validate the model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Post it to the acquiring bank
        PaymentStatus status;
        AcquiringBankResponse abResponse = null;
        try
        {
            abResponse = await _abService.PostPaymentAsync(request.ToAcquiringBankRequest());
            if (abResponse.Authorized)
            {
                status = PaymentStatus.Authorized;
            }
            else
            {
                status = PaymentStatus.Declined;
            }
        }
        catch (PaymentRejectedException ex)
        {
            _logger.LogError(ex, ex.Message);
            status = PaymentStatus.Rejected;
        }

        Guid? id = null;
        try
        {
            // Store the result for future retrieval
            id = (await _paymentsRepository.AddAsync(request.ToPaymentDetails(status,
                abResponse?.AuthorizationCode))).Id;
        }
        catch (Exception ex)
        {
            // This is a critical error as the transaction may have been submitted to the acquiring bank, but the merchant might
            // not have a way to retrieve these details. This kind of error should be alerted to the support team to investigate.
            _logger.LogCritical(ex,
                $"Error when persisting payment details to repository. This means something was submitted to the acquiring bank but may not be stored in the data store. Auth: {abResponse.Authorized}, AuthCode: {abResponse.AuthorizationCode}");
        }

        // Return the result to the merchant
        return new OkObjectResult(new PostPaymentResponse()
        {
            CardNumberLastFour = request.CardNumber.ToLastFour(),
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency,
            Amount = request.Amount,
            Status = status,
            Id = id
        });
    }
}