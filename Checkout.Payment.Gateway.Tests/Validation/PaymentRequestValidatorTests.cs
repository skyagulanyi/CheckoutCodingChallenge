using NUnit.Framework;
using System;
using NSubstitute;
using Checkout.Payment.Entities;
using Checkout.Payment.Gateway.Validation;

namespace Checkout.Payment.Gateway.Tests.Validation
{
    [TestFixture]
    public class PaymentRequestValidatorTests
    {
        private IDateService _dateService = null;

        IPaymentRequestValidator _validator = null;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dateService = Substitute.For<IDateService>();

            _dateService.GetCurrentDate().Returns(new DateTime(2020, 8, 1));
        }

        [SetUp]
        public void SetUp()
        {
            _validator = new PaymentRequestValidator(_dateService);
        }

        [Test]
        [Sequential]
        public void ShouldValidateEndDate([Values("1", "01", "8", "9")] string month, [Values("21", "21", "20", "20")] string year)
        {
            PaymentRequest request = GetValidPaymentRequest();
            request.ExpiryMonthDate = $"{month}/{year}";
            Assert.That(_validator.TryValidate(request, out string msg), Is.EqualTo(true));
            Assert.That(string.IsNullOrEmpty(msg));
        }

        [Test]
        [Sequential]
        public void ShouldFailWhenEndDateIsExpired([Values("7", "08")] string month, [Values("20", "19")] string year)
        {
            PaymentRequest request = GetValidPaymentRequest();
            request.ExpiryMonthDate = $"{month}/{year}";
            Assert.That(_validator.TryValidate(request, out string msg), Is.EqualTo(false));
            Assert.That(msg, Is.EqualTo(PaymentRequestValidator.ValidationMessages.EndDateExpired));
        }

        [Test]
        public void ShouldFailWhenEndDateIsNotValid([Values(null, "", "aa/bb", "aa/01", "01/aa", "0190")] string endMonthYear)
        {
            PaymentRequest request = GetValidPaymentRequest();
            request.ExpiryMonthDate = endMonthYear;
            Assert.That(_validator.TryValidate(request, out string msg), Is.EqualTo(false));
            Assert.That(msg, Is.EqualTo(PaymentRequestValidator.ValidationMessages.EndDateInvalid));
        }

        [Test]
        public void ShouldFailWhenAmountIsOutsideRange([Values(-5d, 0d, 0.09, 10000.01)] double amount)
        {
            PaymentRequest request = GetValidPaymentRequest();
            request.Amount = amount;
            Assert.That(_validator.TryValidate(request, out string msg), Is.EqualTo(false));
            Assert.That(msg, Is.EqualTo(PaymentRequestValidator.ValidationMessages.AmountInvalid));
        }

        [Test]
        public void ShouldFailWhenCvvNotValid([Values(null, "", "1", "12", "ABC", "12C")] string cvv)
        {
            PaymentRequest request = GetValidPaymentRequest();
            request.CVV = cvv;
            Assert.That(_validator.TryValidate(request, out string msg), Is.EqualTo(false));
            Assert.That(msg, Is.EqualTo(PaymentRequestValidator.ValidationMessages.CvvInvalid));
        }


        [Test]
        public void ShouldValidateValidCurrencies([Values("EUR", "GBP", "usd")] string ccy)
        {
            PaymentRequest request = GetValidPaymentRequest();
            request.Ccy = ccy;
            Assert.That(_validator.TryValidate(request, out string msg), Is.EqualTo(true));
        }

        [Test]
        public void ShouldFailWhenCcyNotValid([Values(null, "", "CHF")] string ccy)
        {
            PaymentRequest request = GetValidPaymentRequest();
            request.Ccy = ccy;
            Assert.That(_validator.TryValidate(request, out string msg), Is.EqualTo(false));
            Assert.That(msg, Is.EqualTo(PaymentRequestValidator.ValidationMessages.CcyInvalid));
        }

        [Test]
        public void ShouldFailWhenCardNumberNotValid([Values(null, "", "1111-2222-3333-4444")] string cardNumber)
        {
            PaymentRequest request = GetValidPaymentRequest();
            request.CardNumber = cardNumber;
            Assert.That(_validator.TryValidate(request, out string msg), Is.EqualTo(false));
            Assert.That(msg, Is.EqualTo(PaymentRequestValidator.ValidationMessages.CardNumberInvalid));
        }

        private PaymentRequest GetValidPaymentRequest()
        {
            return new PaymentRequest { ExpiryMonthDate = "01/21", Amount = 1.00d, CVV = "123", Ccy = "EUR", CardNumber = "1111222233334444" };
        }
    }
}