using Checkout.Payment.MockBank.Api.Services;
using System;

namespace Checkout.Payment.MockBank.Api.Domain
{
    internal class UnlimitedAccount : Account
    {
        private readonly Guid _accountId;
        private readonly Card _cardDetails;

        public UnlimitedAccount(Guid accountId, Card cardDetails)
        {
            _accountId = accountId;
            _cardDetails = cardDetails;
        }

        public override Guid AccountId => _accountId;

        public override Card CardDetails => _cardDetails;

        internal override bool CanMakePayment(double amount)
        {
            return true;
        }
    }
}
