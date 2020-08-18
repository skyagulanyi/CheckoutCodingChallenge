using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;

using Microsoft.Extensions.Logging;

namespace Checkout.Payment.Gateway.Clients
{
    public class MockBankClient : IBankClient
    {
        private readonly ILogger<MockBankClient> _logger;
        private readonly Uri _uri;

        public MockBankClient(ILogger<MockBankClient> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this._uri = new Uri($"{configuration.GetSection("MockBankClientUri").Value}/Transaction/Process" );
        }
        public TransactionResponse ProcessTransaction(TransactionRequest request)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    var jsonData = JsonConvert.SerializeObject(request);
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    string response = client.UploadString(_uri, "POST", jsonData);
                    return JsonConvert.DeserializeObject<TransactionResponse>(response);
                }
                catch(WebException e)
                {
                    this._logger.LogError($"Operation {nameof(ProcessTransaction)} errored - status:{e.Status}", e);
                    throw new ApplicationException($"Error processing process transaction request", e);
                }
                catch(Exception e)
                {
                    this._logger.LogError($"Operation {nameof(ProcessTransaction)} errored", e);
                    throw new ApplicationException($"Error processing process transaction request", e);
                }
                
            }
            
        }
    }
}
