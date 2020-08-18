namespace Checkout.Payment.MockBank.Api.Domain
{
    internal class Card
    {
        public Card(string cardNumber, string cvv, string expiry)
        {
            CardNumber = cardNumber;
            Cvv = cvv;
            Expiry = expiry;
        }

        public string CardNumber { get; }
        public string Cvv { get; }
        public string Expiry { get; }
    }
}
