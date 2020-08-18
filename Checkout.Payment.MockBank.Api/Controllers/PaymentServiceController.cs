using System;
using System.Net;
using Checkout.Payment.MockBank.Api.Models;
using Checkout.Payment.MockBank.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Checkout.Payment.MockBank.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentServiceController : ControllerBase
    {
        private readonly IMockPaymentTransactionService paymentTransactionService;
        private readonly ILogger<PaymentServiceController> logger;

        public PaymentServiceController(IMockPaymentTransactionService paymentTransactionService, ILogger<PaymentServiceController> logger)
        {
            this.paymentTransactionService = paymentTransactionService;
            this.logger = logger;
        }

        [HttpPost("Transaction/Process")]
        public IActionResult ProcessTransaction([FromBody]TransactionRequest request)
        {
            try
            {
                var result = paymentTransactionService.ProcessTransaction(request);
                return new OkObjectResult(result);
            }
            catch (Exception e)
            {
                logger.LogError($"Error processing payment request id: {request.PaymentGatewayTxId}", e);
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

        }
    }
}
