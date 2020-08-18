using Checkout.Payment.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;


namespace Checkout.Payment.Gateway.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentGatewayController:ControllerBase
    {
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly ILogger<PaymentGatewayController> _logger;

        public PaymentGatewayController(IPaymentGatewayService paymentGatewayService, ILogger<PaymentGatewayController> logger)
        {
            this._paymentGatewayService = paymentGatewayService;
            this._logger = logger;
        }

        [HttpPost("payment/process")]
        public IActionResult ProcessPayment([FromBody] PaymentRequest request) 
        {
            try
            {
                _logger.LogInformation($"Processing payment MerchantPaymentRequestId:{request.MerchantPaymentRequestId} MerchantId {request.MerchantId} {request.Amount} {request.Ccy}");
                PaymentResponse response = this._paymentGatewayService.ProcessPayment(request);
                _logger.LogInformation($"Processed payment MerchantPaymentRequestId:{request.MerchantPaymentRequestId} MerchantId {request.MerchantId} {request.Amount} {request.Ccy}");
                return new OkObjectResult(response); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing payment MerchantPaymentRequestId:{request.MerchantPaymentRequestId} MerchantId {request.MerchantId} {request.Amount} {request.Ccy}", ex);
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("payment/details")]
        public IActionResult GetPaymentDetails(Guid merchantId, Guid paymentId) 
        {
            try
            {
                _logger.LogInformation($"Processing {nameof(GetPaymentDetails)} merchantId:{merchantId} paymentId:{paymentId}");
                PaymentDetailsResponse response = this._paymentGatewayService.GetPaymentDetails(merchantId, paymentId);

                if (response == null)
                {
                    _logger.LogInformation($"Processed {nameof(GetPaymentDetails)} merchantId:{merchantId} paymentId:{paymentId}. No match found!");
                    return NotFound();
                }

                _logger.LogInformation($"Processed {nameof(GetPaymentDetails)} merchantId:{merchantId} paymentId:{paymentId}");
                return new OkObjectResult(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Operation {nameof(GetPaymentDetails)} errored. merchandId:{merchantId} paymentId:{paymentId}");
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }            
        }
    }
}
