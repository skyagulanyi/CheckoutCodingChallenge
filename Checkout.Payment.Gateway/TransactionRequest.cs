namespace Checkout.Payment.Gateway
{
    using System;
    public class TransactionRequest
    {
        public Guid PaymentGatewayTxId { get; set; }
        public string MerchantName { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryMonthDate { get; set; }
        public double Amount { get; set; }
        public string Ccy { get; set; }
        public string CVV { get; set; }
    }
}