using Checkout.Payment.Entities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using Newtonsoft.Json;
using System.Configuration;
using Checkout.Payment.Client.App.Models;
using Checkout.Payment.Client.App.Commands;

namespace Checkout.Payment.Client.App.ViewModels
{
    public class PaymentGatewayViewModel:INotifyPropertyChanged
    {
        private CardDetails _selectedCard;
        private PreviousPaymentDetails _currentPreviousPayment;
        private Transaction _selectedTransaction;

        private readonly string _apiRootUri;

        public ICommand _makePaymentCommand;
        public PaymentGatewayViewModel()
        {
            _apiRootUri = ConfigurationManager.AppSettings["GatewayApiRoot"];
            Cards = new ObservableCollection<CardDetails>();
            Cards.Add(new CardDetails { CardNumber = "1111111111111111", Description = "Unlimited", Cvv = "111", Expiry = "01/21" });
            Cards.Add(new CardDetails { CardNumber = "2222222222222222", Description = "Overdrawn", Cvv = "111", Expiry = "01/21" });
            Cards.Add(new CardDetails { CardNumber = "3333333333333333", Description = "Unknown", Cvv = "111", Expiry = "01/21" });
            Merchant = new Merchant { Id = new Guid("E0E38966-B96D-4162-86F8-65F78458B02D"), Name = "Ebay" };
            _makePaymentCommand = new RelayCommand(_ => ProcessMakePayment(), _ => SelectedCard != null && Math.Abs(Amount) > 0 );
            Transactions = new ObservableCollection<Transaction>();
        }

        public ObservableCollection<Transaction> Transactions { get; set; }
        public ObservableCollection<CardDetails> Cards { get; set; }
        public double Amount { get; set; }
        public Merchant Merchant { get; }

        public CardDetails SelectedCard
        {
            get { return _selectedCard; }
            set
            {
                _selectedCard = value;
                OnPropertyChanged();
            }
        }

        public Transaction SelectedTransaction
        {
            get { return _selectedTransaction; }
            set
            {
                _selectedTransaction = value;
                OnPropertyChanged();
                RetreivePreviousPayment();
            }
        }

        public PreviousPaymentDetails CurrentPreviousPayment
        {
            get { return _currentPreviousPayment; }
            set
            {
                _currentPreviousPayment = value;
                OnPropertyChanged();
            }
        }

        public ICommand MakePaymentCommand
        {
            get
            {
                return _makePaymentCommand;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName = null) 
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RetreivePreviousPayment()
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    string response = wc.DownloadString($@"{_apiRootUri}/details?merchantId={SelectedTransaction.Merchant.Id}&paymentId={SelectedTransaction.GatewayPaymentId}");

                    PaymentDetailsResponse paymentResponse = JsonConvert.DeserializeObject<PaymentDetailsResponse>(response);

                    CurrentPreviousPayment = new PreviousPaymentDetails
                    {
                        Amount = paymentResponse.Amount,
                        Cvv = paymentResponse.Cvv,
                        Expiry = paymentResponse.ExpiryMonthDate,
                        MaskedCardNumber = paymentResponse.MaskedCardNumber,
                        PaymentId = paymentResponse.PaymentGatewayTxId,
                        Status = paymentResponse.Status,
                        TransactionId = paymentResponse.BankTransactionId,
                        Message = paymentResponse.Message
                    };
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void ProcessMakePayment() 
        {
            using (WebClient wc = new WebClient()) 
            {
                try
                {
                    Guid requestId = Guid.NewGuid();
                    PaymentRequest request = new PaymentRequest();
                    request.Amount = this.Amount;
                    request.CardNumber = SelectedCard.CardNumber;
                    request.Ccy = "EUR";
                    request.CVV = SelectedCard.Cvv;
                    request.ExpiryMonthDate = SelectedCard.Expiry;
                    request.MerchantId = this.Merchant.Id;
                    request.MerchantPaymentRequestId = requestId;

                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    string response = wc.UploadString($@"{_apiRootUri}/process", "POST", Newtonsoft.Json.JsonConvert.SerializeObject(request));

                    PaymentResponse paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(response);

                    Transactions.Add(new Transaction
                    {
                        Amount = this.Amount,
                        Card = new CardDetails
                        {
                            CardNumber = this.SelectedCard.CardNumber,
                            Cvv = this.SelectedCard.Cvv,
                            Description = this.SelectedCard.Description,
                            Expiry = this.SelectedCard.Expiry
                        },
                        GatewayPaymentId = paymentResponse.PaymentGatewayTxId,
                        Merchant = this.Merchant,
                        Message = paymentResponse.Message,
                        Status = paymentResponse.status,
                        RequestPaymentId = paymentResponse.MerchantPaymentRequestId,
                        TransactionId = paymentResponse.TransactionId
                    });
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
    }
}
