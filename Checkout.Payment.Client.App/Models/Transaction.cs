using System;

namespace Checkout.Payment.Client.App.Models
{
    public class Transaction
    {
        public Merchant Merchant { get; set; }
        public CardDetails Card { get; set; }
        public double Amount { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }

        public Guid RequestPaymentId { get; set; }
        public Guid GatewayPaymentId { get; set; }
        public Guid TransactionId { get; set; }
    }
}
