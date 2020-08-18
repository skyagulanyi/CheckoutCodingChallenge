using System;

namespace Checkout.Payment.Gateway
{
    public interface IDateService
    {
        DateTime GetCurrentDate();
    }
}
