using System;

namespace Checkout.Payment.Gateway
{
    public class ProcessedPaymentDetails
    {
        public string CardNumber { get; set; }
        public Guid CheckoutTransactionId { get; set; }
        public string Cvv { get; set; }
        public string ExpiryMonthDate { get; set; }
        public double Amount { get; set; }
        public bool Status { get; set; }
        public Guid MerchantId { get; set; }
        public Guid MerchantPaymentRequestId { get; set; }
        public Guid PaymentGatewayTxId { get; internal set; }
        public Guid BankTransactionId { get; internal set; }
        public string Ccy { get; internal set; }
        public string Message { get; internal set; }
    }
}
