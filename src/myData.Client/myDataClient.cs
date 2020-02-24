using myData.Client.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace myData.Client
{
    public class myDataClient : ImyDataClient
    {
        private readonly string userId;
        private readonly string subscriptionKey;
        private readonly string apiBaseUrl;
        private readonly HttpClient httpClient;

        public myDataClient(string userId, string subscriptionKey, string apiBaseUrl, HttpClient httpClient)
        {
            this.userId = userId;
            this.subscriptionKey = subscriptionKey;
            this.apiBaseUrl = apiBaseUrl;
            this.httpClient = httpClient;
        }

        public RequestedInvoicesDoc RequestInvoices(string mark, string nextPartitionKey = null, string nextRowKey = null)
        {
            return RequestInvoicesAsync(mark, nextPartitionKey, nextRowKey).GetAwaiter().GetResult();
        }

        public async Task<RequestedInvoicesDoc> RequestInvoicesAsync(string mark, string nextPartitionKey = null, string nextRowKey = null)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["mark"] = mark;
            if (!string.IsNullOrWhiteSpace(nextPartitionKey))
            {
                queryString["nextPartitionKey"] = nextPartitionKey;
            }
            if (!string.IsNullOrWhiteSpace(nextRowKey))
            {
                queryString["nextRowKey"] = nextRowKey;
            }

            var request = CreateHttpRequestMessage("RequestInvoices?" + queryString, HttpMethod.Get);
            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var responseDeserializer = new XmlSerializer(typeof(RequestedInvoicesDoc));
                var responseDoc = (RequestedInvoicesDoc)responseDeserializer.Deserialize(await response.Content.ReadAsStreamAsync());

                return responseDoc;
            }
        }

        public RequestedInvoicesDoc RequestIssuerInvoices(string mark, string nextPartitionKey = null, string nextRowKey = null)
        {
            throw new NotImplementedException();
        }

        public Task<RequestedInvoicesDoc> RequestIssuerInvoicesAsync(string mark, string nextPartitionKey = null, string nextRowKey = null)
        {
            throw new NotImplementedException();
        }

        public ResponseDoc SendExpensesClassification(ExpensesClassificationsDoc expensesClassificationsDoc)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDoc> SendExpensesClassificationAsync(ExpensesClassificationsDoc expensesClassificationsDoc)
        {
            throw new NotImplementedException();
        }

        public ResponseDoc SendIncomeClassification(IncomeClassificationsDoc incomeClassificationsDoc)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDoc> SendIncomeClassificationAsync(IncomeClassificationsDoc incomeClassificationsDoc)
        {
            throw new NotImplementedException();
        }

        public ResponseDoc SendInvoices(InvoicesDoc invoicesDoc)
        {
            return SendInvoicesAsync(invoicesDoc).GetAwaiter().GetResult();
        }

        public async Task<ResponseDoc> SendInvoicesAsync(InvoicesDoc invoicesDoc)
        {
            var requestSerializer = new XmlSerializer(typeof(InvoicesDoc));
            var responseDeserializer = new XmlSerializer(typeof(ResponseDoc));
            using (var stream = new MemoryStream())
            {
                requestSerializer.Serialize(stream, invoicesDoc);
                var httpContent = new ByteArrayContent(stream.ToArray());
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");
                var request = CreateHttpRequestMessage("SendInvoices", HttpMethod.Post, httpContent);
                using (var response = await httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var responseDoc = (ResponseDoc)responseDeserializer.Deserialize(await response.Content.ReadAsStreamAsync());

                    return responseDoc;
                }
            }
        }

        private HttpRequestMessage CreateHttpRequestMessage(string apiMethod, HttpMethod method, HttpContent httpContent = null)
        {
            var uri = new Uri(new Uri(apiBaseUrl), apiMethod);
            var requestMessage = new HttpRequestMessage(method, uri);
            if (httpContent != null)
            {
                requestMessage.Content = httpContent;
            }
            requestMessage.Headers.Add("aade-user-id", userId);
            requestMessage.Headers.Add("ocp-apim-subscription-key", subscriptionKey);

            return requestMessage;
        }
    }
}
