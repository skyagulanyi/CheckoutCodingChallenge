using System;

namespace Checkout.Payment.Entities
{
    public class PaymentRequest
    {
        public Guid MerchantId { get; set; }
        public Guid MerchantPaymentRequestId { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryMonthDate { get; set; }
        public double Amount { get; set; }
        public string Ccy { get; set; }
        public string CVV { get; set; }
    }
}