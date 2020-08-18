using Checkout.Payment.Entities;

namespace Checkout.Payment.Gateway.Validation
{
    public interface IPaymentRequestValidator
    {
        bool TryValidate(PaymentRequest request, out string msg);
    }
}