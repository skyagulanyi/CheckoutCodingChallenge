using NUnit.Framework;
using System;
using NSubstitute;
using Checkout.Payment.Gateway.Clients;
using Checkout.Payment.Entities;
using Checkout.Payment.Gateway.Validation;
using NSubstitute.Core;
using Microsoft.Extensions.Logging;

namespace Checkout.Payment.Gateway.Tests
{
    [TestFixture]
    public class PaymentGatewayServiceTests
    {
        private IDateService _dateService = null;
        IBankClient _bankClient = null;
        IPaymentGatewayService _service = null;
        IPaymentRequestValidator _validator = null;
        ILogger<PaymentGatewayService> _logger = null;
        private Guid _merchantId = new Guid("E0E38966-B96D-4162-86F8-65F78458B02D");
        private string _merchantName = "Ebay";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dateService = Substitute.For<IDateService>();
            _logger = Substitute.For<ILogger<PaymentGatewayService>>();
            _validator = new PaymentRequestValidator(_dateService);
            
            _dateService.GetCurrentDate().Returns(new DateTime(2020,8,1));
        }

        [SetUp]
        public void SetUp() 
        {
            _bankClient = Substitute.For<IBankClient>();
            _service = new PaymentGatewayService(_bankClient, _validator, _logger);
        }

        [Test]
        public void ShouldProcessValidMessage()
        {
            PaymentRequest request = GetValidPaymentRequest();
            _bankClient.ProcessTransaction(Arg.Any<TransactionRequest>()).Returns(new TransactionResponse {Status = true });
            var response = _service.ProcessPayment(request);
            Assert.That(response.status, Is.EqualTo(true));
            Assert.That(string.IsNullOrEmpty(response.Message));
        }

        [Test]
        public void ShouldNotProcessValidMessage()
        {
            PaymentRequest request = GetValidPaymentRequest();
            request.Ccy = null;
            _bankClient.DidNotReceive().ProcessTransaction(Arg.Any<TransactionRequest>());
            var response = _service.ProcessPayment(request);
            Assert.That(response.status, Is.EqualTo(false));
            Assert.That(response.Message, Is.EqualTo(PaymentRequestValidator.ValidationMessages.CcyInvalid));
        }

        [Test]
        public void ShouldReturnProcessedSuccessPaymentFromCache() 
        {
            Guid bankingTransactionId = Guid.NewGuid();
            Guid merchantPaymentRequestId = Guid.NewGuid();
            PaymentRequest request = GetValidPaymentRequest();

            request.MerchantPaymentRequestId = merchantPaymentRequestId;

            var response = new TransactionResponse
            {
                BankTransactionId = bankingTransactionId,
                Status = true,
            };
            _bankClient.ProcessTransaction(Arg.Is<TransactionRequest>(x => x.CardNumber == request.CardNumber && x.Amount == request.Amount)).Returns(response);
            var txResponse = _service.ProcessPayment(request);
            PaymentDetailsResponse details = _service.GetPaymentDetails(_merchantId, txResponse.PaymentGatewayTxId);

            AssetProcessDetails(details, request, txResponse, merchantPaymentRequestId, bankingTransactionId);
        }

        [Test]
        public void ShouldReturnProcessedFailPaymentFromCache()
        {
            Guid merchantPaymentRequestId = Guid.NewGuid();
            PaymentRequest request = GetValidPaymentRequest();

            request.MerchantPaymentRequestId = merchantPaymentRequestId;
            string message = "Insufficient Funds";

            var response = new TransactionResponse
            {
                Status = false,
                Message = message
            };
            _bankClient.ProcessTransaction(Arg.Is<TransactionRequest>(x => x.CardNumber == request.CardNumber && x.Amount == request.Amount)).Returns(response);
            PaymentResponse txResponse = _service.ProcessPayment(request);
            PaymentDetailsResponse details = _service.GetPaymentDetails(_merchantId, txResponse.PaymentGatewayTxId);

            AssetProcessDetails(details, request, txResponse, merchantPaymentRequestId, Guid.Empty, false, message);
        }

        [Test]
        public void ShouldNotReturnFaultedPaymentFromCache()
        {
            Guid paymentId = Guid.Empty;
            try
            {
                Guid merchantPaymentRequestId = Guid.NewGuid();
                PaymentRequest request = GetValidPaymentRequest();
                

                request.MerchantPaymentRequestId = merchantPaymentRequestId;
                string message = "Insufficient Funds";

                var response = new TransactionResponse
                {
                    Status = false,
                    Message = message
                };
                _bankClient.ProcessTransaction(Arg.Do<TransactionRequest>(x => paymentId = x.PaymentGatewayTxId)).Returns(x => throw new Exception("Network Error"));
                PaymentResponse txResponse = _service.ProcessPayment(request);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.That(e.Message.ToLowerInvariant().Contains("network"));
            }
            
            PaymentDetailsResponse details = _service.GetPaymentDetails(_merchantId, paymentId);
            Assert.That(paymentId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(details, Is.Null);
        }

        private void AssetProcessDetails(PaymentDetailsResponse details, PaymentRequest request, PaymentResponse txResponse, Guid expectedMerchantPaymentRequestId, 
            Guid bankTransactionId, bool expectedIsSuccess = true, string expectedErrorMessage = null) 
        {
            //verify status
            Assert.That(details.Status, Is.EqualTo(expectedIsSuccess));

            //verify ids
            Assert.That(details.MerchantId, Is.EqualTo(_merchantId));
            Assert.That(details.MerchantPaymentRequestId, Is.EqualTo(expectedMerchantPaymentRequestId));
            Assert.That(details.PaymentGatewayTxId, Is.EqualTo(txResponse.PaymentGatewayTxId));
            Assert.That(details.BankTransactionId, Is.EqualTo(bankTransactionId));

            //other fields
            Assert.That(details.Amount, Is.EqualTo(request.Amount));
            Assert.That(details.Cvv, Is.EqualTo(request.CVV));
            Assert.That(details.ExpiryMonthDate, Is.EqualTo(request.ExpiryMonthDate));
            Assert.That(details.Ccy, Is.EqualTo(request.Ccy));

            //assert masked card number
            Assert.That(details.MaskedCardNumber.StartsWith("XXXX"));
            Assert.That(details.MaskedCardNumber.EndsWith("XXXX"));
            Assert.That(request.CardNumber.Contains(details.MaskedCardNumber.Replace("X", string.Empty)));

            Assert.That(details.Message, Is.EqualTo(expectedErrorMessage));
        }

        private TransactionResponse response(CallInfo arg)
        {
            throw new NotImplementedException();
        }

        private PaymentRequest GetValidPaymentRequest()
        {
            return new PaymentRequest { ExpiryMonthDate = "01/21", Amount = 1.00d, CVV = "123", Ccy = "EUR", CardNumber= "1111222233334444", MerchantId= _merchantId };
        }
    }
}