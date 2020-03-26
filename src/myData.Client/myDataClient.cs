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

        public ResponseDoc CancelInvoice(string mark)
        {
            return CancelInvoiceAsync(mark).GetAwaiter().GetResult();
        }

        public async Task<ResponseDoc> CancelInvoiceAsync(string mark)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["mark"] = mark;
            var request = CreateHttpRequestMessage("CancelInvoice?" + queryString, HttpMethod.Post);
            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return await ParseXmlResponse<ResponseDoc>(response);
            }
        }

        public RequestedDoc RequestDocs(long? mark = null)
        {
            return RequestDocsAsync(mark).GetAwaiter().GetResult();
        }

        public async Task<RequestedDoc> RequestDocsAsync(long? mark = null)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            if (mark != null)
            {
                queryString["mark"] = mark.ToString();
            }

            var request = CreateHttpRequestMessage("RequestDocs?" + queryString, HttpMethod.Get);
            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return await ParseXmlResponse<RequestedDoc>(response);
            }
        }

        public RequestedDoc RequestTransmittedDocs(long mark)
        {
            return RequestTransmittedDocsAsync(mark).GetAwaiter().GetResult();
        }

        public async Task<RequestedDoc> RequestTransmittedDocsAsync(long mark)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["mark"] = mark.ToString();
            var request = CreateHttpRequestMessage("RequestTransmittedDocs?" + queryString, HttpMethod.Get);
            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var responseDeserializer = new XmlSerializer(typeof(RequestedDoc));
                return await ParseXmlResponse<RequestedDoc>(response);
            }
        }

        public ResponseDoc SendExpensesClassification(ExpensesClassificationsDoc expensesClassificationsDoc)
        {
            return SendExpensesClassificationAsync(expensesClassificationsDoc).GetAwaiter().GetResult();
        }

        public async Task<ResponseDoc> SendExpensesClassificationAsync(ExpensesClassificationsDoc expensesClassificationsDoc)
        {
            var httpContent = CreateXmlRequestBody(expensesClassificationsDoc);
            var request = CreateHttpRequestMessage("SendExpensesClassification", HttpMethod.Post, httpContent);
            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return await ParseXmlResponse<ResponseDoc>(response);
            }

        }

        public ResponseDoc SendIncomeClassification(IncomeClassificationsDoc incomeClassificationsDoc)
        {
            return SendIncomeClassificationAsync(incomeClassificationsDoc).GetAwaiter().GetResult();
        }

        public async Task<ResponseDoc> SendIncomeClassificationAsync(IncomeClassificationsDoc incomeClassificationsDoc)
        {
            var httpContent = CreateXmlRequestBody(incomeClassificationsDoc);
            var request = CreateHttpRequestMessage("SendIncomeClassification", HttpMethod.Post, httpContent);
            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return await ParseXmlResponse<ResponseDoc>(response);
            }
        }

        public ResponseDoc SendInvoices(InvoicesDoc invoicesDoc)
        {
            return SendInvoicesAsync(invoicesDoc).GetAwaiter().GetResult();
        }

        public async Task<ResponseDoc> SendInvoicesAsync(InvoicesDoc invoicesDoc)
        {
            var httpContent = CreateXmlRequestBody(invoicesDoc);
            var request = CreateHttpRequestMessage("SendInvoices", HttpMethod.Post, httpContent);
            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return await ParseXmlResponse<ResponseDoc>(response);
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

        private async Task<T> ParseXmlResponse<T>(HttpResponseMessage response)
        {
            var responseDeserializer = new XmlSerializer(typeof(T));
            var responseStr = await response.Content.ReadAsStringAsync();
            try
            {
                var responseDoc = (T)responseDeserializer.Deserialize(new StringReader(responseStr));
                return responseDoc;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to parse response: " + responseStr, ex);
            }
        }

        private HttpContent CreateXmlRequestBody<T>(T requestData)
        {
            var requestSerializer = new XmlSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                requestSerializer.Serialize(stream, requestData);
                var httpContent = new ByteArrayContent(stream.ToArray());
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");
                return httpContent;
            }
        }
    }
}
