using Checkout.Payment.MockBank.Api.Domain;
using Checkout.Payment.MockBank.Api.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Checkout.Payment.MockBank.Api.Services
{
    internal class MockPaymentTransactionService : IMockPaymentTransactionService
    {
        private readonly string _unlimitedAccountCardNumber = "1111111111111111";
        private readonly string _overdrawnAccountCardNumber = "2222222222222222";

        private readonly IDictionary<string, Account> _accountsLookupByCardNo;
        private readonly ILogger<MockPaymentTransactionService> logger;

        public MockPaymentTransactionService(ILogger<MockPaymentTransactionService> logger)
        {
            _accountsLookupByCardNo = new Dictionary<string, Account>
            {
                {_unlimitedAccountCardNumber, new UnlimitedAccount(new Guid("D483E87E-A17B-4911-A4E7-8CD131DC69CE"), new Card(_unlimitedAccountCardNumber,"111","01/21")) },
                {_overdrawnAccountCardNumber, new OverdrawnAccount(new Guid("914871E8-809B-4F42-97E2-BA130DCE7FA6"), new Card(_overdrawnAccountCardNumber, "222", "02/22")) }
            };
            this.logger = logger;
        }

        public TransactionResponse ProcessTransaction(TransactionRequest request)
        {
            logger.LogInformation($"Received payment request {request.MerchantName} {request.Amount} {request.Ccy}");
            string msg = null;
            if (!_accountsLookupByCardNo.ContainsKey(request.CardNumber))
            {
                msg = "Account Not Found";
                logger.LogInformation($"Processed payment request {request.MerchantName} {request.Amount} {request.Ccy}: {msg}");
                return new TransactionResponse { Status = false, Message = msg, PaymentGatewayTxId = request.PaymentGatewayTxId };
            }

            Account account = _accountsLookupByCardNo[request.CardNumber];

            TransactionResponse response;
            if (account.CanMakePayment(request.Amount))
            {
                response = new TransactionResponse { Status = true, PaymentGatewayTxId = request.PaymentGatewayTxId, BankTransactionId = Guid.NewGuid() };
            }
            else
            {
                msg = "Insufficient Funds";
                response = new TransactionResponse { Status = false, Message = msg, PaymentGatewayTxId = request.PaymentGatewayTxId };
            }

            logger.LogInformation($"Processed payment request {request.MerchantName} {request.Amount} {request.Ccy} {(string.IsNullOrWhiteSpace(msg) ? "" : ":")} {msg}");
            return response;
        }
    }
}
