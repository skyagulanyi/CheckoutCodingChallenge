using System;

namespace Checkout.Payment.Entities
{
    public class PaymentResponse
    {
        public Guid MerchantId { get; set; }
        public Guid MerchantPaymentRequestId { get; set; }
        public Guid TransactionId { get; set; }
        public bool status { get; set; }
        public string Message { get; set; }
        public Guid PaymentGatewayTxId { get; set; }
    }
}