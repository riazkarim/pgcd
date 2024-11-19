using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Helpers;
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
    public async Task<ActionResult<PostPaymentResponse?>> PostPaymentAsync([FromBody] PostPaymentRequest request)
    {
        PaymentStatus status = PaymentStatus.Rejected; //Set this to a safe default state
        AcquiringBankResponse abResponse = null;
        Guid? id = null;
        
        //Validate the model
        if (!ModelState.IsValid)
        {
            // The submitted data in invalid. Set the return status to rejected.
            status = PaymentStatus.Rejected;
        }
        else
        {
            // The model is valid, so post it to the acquiring bank and persist it.
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
            catch (AcquiringBankUnavailableException ex)
            {
                _logger.LogCritical(ex, ex.Message);
                // If we can't reach the acquiring bank, we return the error to the merchant, as this is a critical non-recoverable failure
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

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
                // We should still return the acquiring bank response to the user so they can be alerted too on the status of their transaction.
                // They won't have an ID in this case for now, until our dev team fixes the data.
                _logger.LogCritical(ex,
                    $"Error when persisting payment details to repository. This means something was submitted to the acquiring bank but may not be stored in the data store. Auth: {abResponse.Authorized}, AuthCode: {abResponse.AuthorizationCode}");
            }
        }

        // Return the result to the merchant
        return new OkObjectResult(request.ToPostPaymentResponse(status, id));
    }
}