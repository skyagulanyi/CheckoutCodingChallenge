namespace Checkout.Payment.MockBank.Api.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class TransactionRequest
    {
        [Required]
        public Guid PaymentGatewayTxId { get; set; }
        [Required]
        public string MerchantName { get; set; }
        [Required, StringLength(maximumLength:16,ErrorMessage ="Card Number must be 16 chars", MinimumLength =16)]
        public string CardNumber { get; set; }
        [Required]
        public string ExpiryMonthDate { get; set; }
        [Required, Range(0.05d,double.MaxValue, ErrorMessage ="Amount must be positive")]
        public double Amount { get; set; }
        [Required, StringLength(maximumLength: 3, ErrorMessage = "Ccy must be 3 chars", MinimumLength = 3)]
        public string Ccy { get; set; }
        [Required, StringLength(maximumLength: 3, ErrorMessage = "CVV must be 3 chars", MinimumLength = 3)]
        public string CVV { get; set; }
    }
}