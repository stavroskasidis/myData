using myData.Client.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace myData.Client
{
    public class myDataClient
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

        public void SendInvoices(InvoicesDoc invoicesDoc)
        {
            SendInvoicesAsync(invoicesDoc).GetAwaiter().GetResult();
        }

        public async Task SendInvoicesAsync(InvoicesDoc invoicesDoc)
        {
            var xmlSerializer = new XmlSerializer(typeof(InvoicesDoc));
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, invoicesDoc);
                var xml = textWriter.ToString();
                var httpContent = new StringContent(xml, Encoding.UTF8, "application/xml");
                var request = CreateHttpRequestMessage("SendInvoices", HttpMethod.Post, httpContent);
                using (var response = await httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        private HttpRequestMessage CreateHttpRequestMessage(string apiMethod, HttpMethod method, HttpContent httpContent)
        {
            var uri = new Uri(new Uri(apiBaseUrl), apiMethod);
            var requestMessage = new HttpRequestMessage(method, uri);
            requestMessage.Headers.Add("aade-user-id", userId);
            requestMessage.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            requestMessage.Content = httpContent;

            return requestMessage;
        }
    }
}
