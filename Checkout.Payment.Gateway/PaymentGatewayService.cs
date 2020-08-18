using Checkout.Payment.Entities;
using Checkout.Payment.Gateway.Clients;
using Checkout.Payment.Gateway.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Checkout.Payment.Gateway
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        /// <summary>
        /// TODO - Move processed payment details and requests to DB
        /// </summary>
        private readonly ConcurrentDictionary<ProcessedPaymentKey, ProcessedPaymentDetails> _processedPaymentCache = new ConcurrentDictionary<ProcessedPaymentKey, ProcessedPaymentDetails>();
        
        private readonly IBankClient _bankClient;
        private readonly IPaymentRequestValidator _paymentRequestValidator;
        private readonly ILogger<PaymentGatewayService> _logger;

        /// <summary>
        /// TODO - merchant registration to DB
        /// </summary>
        private readonly IDictionary<Guid,string> _registeredMerchants = new Dictionary<Guid,string> { { new Guid("E0E38966-B96D-4162-86F8-65F78458B02D") , "Ebay"} };

        public PaymentGatewayService(IBankClient bankClient, IPaymentRequestValidator paymentRequestValidator, ILogger<PaymentGatewayService> logger)
        {
            this._bankClient = bankClient;
            this._paymentRequestValidator = paymentRequestValidator;
            this._logger = logger;
        }

        public PaymentDetailsResponse GetPaymentDetails(Guid merchantId, Guid paymentGatewayTxId)
        {
            try
            {
                _logger.LogInformation($"MerchantId:{merchantId} PaymentGatewayTxId:{paymentGatewayTxId} Retreiving payment details");
                if (_processedPaymentCache.TryGetValue(new ProcessedPaymentKey(merchantId, paymentGatewayTxId), out ProcessedPaymentDetails processedPaymentDetails))
                {
                    _logger.LogInformation($"MerchantId:{merchantId} PaymentGatewayTxId:{paymentGatewayTxId} Found payment details");
                    return new PaymentDetailsResponse
                    {
                        MaskedCardNumber = processedPaymentDetails.CardNumber,
                        Cvv = processedPaymentDetails.Cvv,
                        Ccy = processedPaymentDetails.Ccy,
                        ExpiryMonthDate = processedPaymentDetails.ExpiryMonthDate,
                        Amount = processedPaymentDetails.Amount,
                        Status = processedPaymentDetails.Status,
                        MerchantId = processedPaymentDetails.MerchantId,
                        MerchantPaymentRequestId = processedPaymentDetails.MerchantPaymentRequestId,
                        PaymentGatewayTxId = processedPaymentDetails.PaymentGatewayTxId,
                        BankTransactionId = processedPaymentDetails.BankTransactionId,
                        Message = processedPaymentDetails.Message
                    };
                }

                _logger.LogWarning($"MerchantId:{merchantId} PaymentGatewayTxId:{paymentGatewayTxId} Payment details not found!");
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError($"MerchantId:{merchantId} PaymentGatewayTxId:{paymentGatewayTxId} Error retreiving payment details",e);
                throw;
            }
        }

        public PaymentResponse ProcessPayment(PaymentRequest request)
        {
            try
            {
                _logger.LogInformation($"MerchantId:{request.MerchantId} MerchantPaymentRequestId:{request.MerchantPaymentRequestId} Validating request");

                if (!_registeredMerchants.ContainsKey(request.MerchantId))
                {
                    _logger.LogWarning($"MerchantId:{request.MerchantId} MerchantPaymentRequestId:{request.MerchantPaymentRequestId} Merchant Unregisterd");
                    return new PaymentResponse { status = false, Message = "Merchant Unregistered", MerchantId = request.MerchantId };
                }

                if (!_paymentRequestValidator.TryValidate(request, out string msg))
                {
                    return new PaymentResponse { status = false, Message = msg, MerchantId = request.MerchantId };
                }

                _logger.LogInformation($"MerchantId:{request.MerchantId} MerchantPaymentRequestId:{request.MerchantPaymentRequestId} Processing request");
                
                Guid paymentId = Guid.NewGuid();
                TransactionResponse txResponse = _bankClient.ProcessTransaction(new TransactionRequest
                {
                    Amount = request.Amount,
                    CardNumber = request.CardNumber,
                    Ccy = request.Ccy,
                    CVV = request.CVV,
                    ExpiryMonthDate = request.ExpiryMonthDate,
                    PaymentGatewayTxId = paymentId,
                    MerchantName = _registeredMerchants[request.MerchantId]
                }
                );

                _logger.LogInformation($"MerchantId:{request.MerchantId} MerchantPaymentRequestId:{request.MerchantPaymentRequestId} Storing payment response from bank");
                _processedPaymentCache[new ProcessedPaymentKey(request.MerchantId, paymentId)] = new ProcessedPaymentDetails
                {
                    Amount = request.Amount,
                    Cvv = request.CVV,
                    Ccy = request.Ccy,
                    ExpiryMonthDate = request.ExpiryMonthDate,
                    MerchantId = request.MerchantId,
                    Status = txResponse.Status,
                    CheckoutTransactionId = paymentId,
                    MerchantPaymentRequestId = request.MerchantPaymentRequestId,
                    //Masking CardNumber here in case crash damp is access wherein complete card numbere may be accessible
                    CardNumber =  MaskPaymentCardNumber(request.CardNumber),
                    PaymentGatewayTxId = paymentId,
                    BankTransactionId = txResponse.BankTransactionId,
                    Message = txResponse.Message
                };

                
                _logger.LogInformation($"MerchantId:{request.MerchantId} MerchantPaymentRequestId:{request.MerchantPaymentRequestId} Processing complete");
                return new PaymentResponse
                {
                    status = txResponse.Status,
                    MerchantId = request.MerchantId,
                    TransactionId = txResponse.BankTransactionId,
                    PaymentGatewayTxId = paymentId,
                    Message = txResponse.Message,
                    MerchantPaymentRequestId = request.MerchantPaymentRequestId
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"MerchantId:{request.MerchantId} MerchantPaymentRequestId:{request.MerchantPaymentRequestId} Error processing request", e);
                throw;
            }
        }

        internal string MaskPaymentCardNumber(string cardNumber) 
        {
            return $"XXXX{cardNumber.Substring(4, 12)}XXXX";
        }
    }
}
