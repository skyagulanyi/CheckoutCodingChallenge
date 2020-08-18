using Checkout.Payment.Entities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Checkout.Payment.Gateway.Validation
{
    public class PaymentRequestValidator : IPaymentRequestValidator
    {
        private readonly double minAmount = 0.10;
        private readonly double maxAmount = 10_000;
        private readonly Regex _expiryMonthDateRegEx = new Regex(@"[0-9]{1,2}/[0-9]{1,2}", RegexOptions.Compiled);
        private readonly Regex _cvvRegEx = new Regex(@"[0-9]{3}", RegexOptions.Compiled);
        private readonly Regex _cardNumberRegEx = new Regex(@"[0-9]{16}", RegexOptions.Compiled);
        private readonly HashSet<string> _supportedCurrencies = new HashSet<string>(new string[] { "USD", "EUR", "GBP" }, StringComparer.InvariantCultureIgnoreCase);
        private IDateService _dateService;

        public PaymentRequestValidator(IDateService dateService)
        {
            this._dateService = dateService;
        }

        public bool TryValidate(PaymentRequest request, out string msg)
        {
            msg = null;
            if (request.Amount < minAmount || request.Amount > maxAmount)
            {
                msg = ValidationMessages.AmountInvalid;
                return false;
            }

            if (!ValidateCardNumber(request.CardNumber, ref msg))
            {
                return false;
            }

            if (!ValidatePaymentEndDate(request.ExpiryMonthDate, ref msg))
            {
                return false;
            }

            if (!ValidateCvv(request.CVV, ref msg))
            {
                return false;
            }

            if (!_supportedCurrencies.Contains(request.Ccy))
            {
                msg = ValidationMessages.CcyInvalid;
                return false;
            }

            return true;
        }

        private bool ValidateCardNumber(string cardNumber, ref string msg)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || !_cardNumberRegEx.IsMatch(cardNumber))
            {
                msg = ValidationMessages.CardNumberInvalid;
                return false;
            }
            return true;
        }

        private bool ValidateCvv(string cvv, ref string msg)
        {
            if (string.IsNullOrWhiteSpace(cvv) || !_cvvRegEx.IsMatch(cvv))
            {
                msg = ValidationMessages.CvvInvalid;
                return false;
            }

            return true;
        }

        private bool ValidatePaymentEndDate(string expiryMonthDate, ref string msg)
        {
            if (string.IsNullOrWhiteSpace(expiryMonthDate) || !_expiryMonthDateRegEx.IsMatch(expiryMonthDate))
            {
                msg = ValidationMessages.EndDateInvalid;
                return false;
            }

            string[] expiryMonthDateParts = expiryMonthDate.Split('/');

            int year = int.Parse(expiryMonthDateParts[1]);
            DateTime curDate = _dateService.GetCurrentDate();
            int currentYear = curDate.Year % 100;


            if (year > currentYear)
            {
                return true;
            }

            if (year < currentYear)
            {
                msg = ValidationMessages.EndDateExpired;
                return false;
            }

            if (int.Parse(expiryMonthDateParts[0]) < curDate.Month)
            {
                msg = ValidationMessages.EndDateExpired;
                return false;
            }

            return true;
        }

        public static class ValidationMessages
        {
            public static string EndDateExpired = "Payment 'EndDate' expired";
            public static string EndDateInvalid = "Payment 'EndDate' invalid";
            public static string AmountInvalid = "Payment 'Amount' invalid";
            public static string CvvInvalid = "Payment 'CVV' invalid";
            public static string CcyInvalid = "Payment 'Ccy' invalid or not supported";
            public static string CardNumberInvalid = "Payment 'CardNumber' invalid";
        }
    }
}
