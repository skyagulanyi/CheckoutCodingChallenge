using System;

namespace Checkout.Payment.Gateway
{
    public class DateService : IDateService
    {
        public DateTime GetCurrentDate()
        {
            return DateTime.UtcNow;
        }
    }
}
