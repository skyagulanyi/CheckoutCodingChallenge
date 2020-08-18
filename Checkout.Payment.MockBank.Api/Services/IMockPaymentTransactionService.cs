using Checkout.Payment.MockBank.Api.Models;

namespace Checkout.Payment.MockBank.Api.Services
{
    public interface IMockPaymentTransactionService
    {
        TransactionResponse ProcessTransaction(TransactionRequest request);
    }
}