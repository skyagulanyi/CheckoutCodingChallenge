using Checkout.Payment.Entities;
using System;

namespace Checkout.Payment.Gateway
{
    public interface IPaymentGatewayService
    {
        PaymentResponse ProcessPayment(PaymentRequest request);
        PaymentDetailsResponse GetPaymentDetails(Guid merchantId, Guid paymentId);
    }
}
