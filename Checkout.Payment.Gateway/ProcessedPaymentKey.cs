using System;
using System.Collections.Generic;

namespace Checkout.Payment.Gateway
{
    public class ProcessedPaymentKey
    {
        public ProcessedPaymentKey(Guid merchantId, Guid paymentId)
        {
            MerchantId = merchantId;
            PaymentId = paymentId;
        }

        public Guid MerchantId { get; private set; }
        public Guid PaymentId { get; private set; }

        public override bool Equals(object obj)
        {
            return obj is ProcessedPaymentKey key &&
                   MerchantId == key.MerchantId &&
                   PaymentId.Equals(key.PaymentId);
        }

        public override int GetHashCode()
        {
            var hashCode = 71857844;
            hashCode = hashCode * -1521134295 + MerchantId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(PaymentId);
            return hashCode;
        }
    }
}
