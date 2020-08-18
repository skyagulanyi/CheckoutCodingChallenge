using System;

namespace Checkout.Payment.Client.App.Models
{
    public class PreviousPaymentDetails
    {
        public Guid PaymentId { get; set; }
        public Guid TransactionId { get; set; }
        public string MaskedCardNumber { get; set; }
        public string Cvv { get; set; }

        public string Expiry { get; set; }
        public bool Status { get; set; }
        public double Amount { get; set; }
        public string Message { get; internal set; }
    }
}
