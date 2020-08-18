namespace Checkout.Payment.Gateway.Clients
{
    public interface IBankClient
    {
        TransactionResponse ProcessTransaction(TransactionRequest request);
    }
}
