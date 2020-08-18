namespace Checkout.Payment.Client.App.Models
{
    public class CardDetails
    {
        public string Description { get; set; }
        public string CardNumber { get; set; }
        public string Cvv { get; set; }
        public string Expiry { get; set; }
    }
}
