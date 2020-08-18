using Checkout.Payment.MockBank.Api.Services;
using System;

namespace Checkout.Payment.MockBank.Api.Domain
{
    internal abstract class Account
    {
        public abstract Guid AccountId { get; }
        public abstract Card CardDetails { get; }

        internal abstract bool CanMakePayment(double amount);
    }
}
