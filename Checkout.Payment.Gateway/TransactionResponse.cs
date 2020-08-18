using System;

namespace Checkout.Payment.Gateway
{
    public class TransactionResponse
    {
        public Guid BankTransactionId { get; set; }
        public Guid PaymentGatewayTxId { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}