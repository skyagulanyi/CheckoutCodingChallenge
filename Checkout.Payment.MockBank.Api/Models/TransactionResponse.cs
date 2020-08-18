using System;

namespace Checkout.Payment.MockBank.Api.Models
{
    public class TransactionResponse
    {
        public Guid BankTransactionId { get; set; }
        public Guid PaymentGatewayTxId { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}