using System;

namespace Checkout.Payment.MockBank.Api.Domain
{
    internal class OverdrawnAccount : Account
    {
        private readonly Guid _accountId;
        private readonly Card _cardDetails;

        public OverdrawnAccount(Guid accountId, Card cardDetails)
        {
            _accountId = accountId;
            _cardDetails = cardDetails;
        }

        public override Guid AccountId => _accountId;

        public override Card CardDetails => _cardDetails;

        internal override bool CanMakePayment(double amount)
        {
            return false;
        }
    }
}
