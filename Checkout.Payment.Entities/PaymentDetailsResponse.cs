using System;

namespace Checkout.Payment.Entities
{
    public class PaymentDetailsResponse
    {
        public Guid MerchantPaymentRequestId { get; set; }
        public Guid MerchantId { get; set; }
        public bool Status { get; set; }
        public double Amount { get; set; }
        public string ExpiryMonthDate { get; set; }
        public string MaskedCardNumber { get; set; }
        public string Cvv { get; set; }
        public string Ccy { get; set; }
        public string Message { get; set; }
        public Guid PaymentGatewayTxId { get; set; }
        public Guid BankTransactionId { get; set; }
    }
}