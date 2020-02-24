using myData.Client;
using myData.Client.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace myData.ConsoleTest.net45
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Workaround for .net 4.5 ssl version
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            var httpClient = new HttpClient();
            var client = new myDataClient("stavroskasidis", "eaa74da20e854172829f6cb9fa2cb76e", "https://mydata-dev.azure-api.net", httpClient);

            var invoicesDoc = new InvoicesDoc();
            invoicesDoc.invoice = new AadeBookInvoiceType[]
            {
                new AadeBookInvoiceType
                {
                    invoiceHeader = new InvoiceHeaderType
                    {
                        series = "SERIES 1",
                        aa = 1111,
                        issueDate = DateTime.Now,
                        invoiceType = InvoiceType.Item111  // ΑΛΠ
                    },
                    invoiceDetails = new InvoiceRowType[]
                    {
                        new InvoiceRowType
                        {
                            lineNumber = 1,
                            netValue = 5,
                            vatCategory = 1 // 24%
                        },
                        new InvoiceRowType
                        {
                            lineNumber = 2,
                            netValue = 10,
                            vatCategory = 2 // 13%
                        }
                    },
                    invoiceSummary = new InvoiceSummaryType
                    {
                        totalNetValue = 15,
                        totalVatAmount = (5 * 0.24m + 10 * 0.13m),
                        totalGrossValue = 15 + (5 * 0.24m + 10 * 0.13m)
                    }
                }
            };

            try
            {
                var response = await client.SendInvoicesAsync(invoicesDoc);
                Console.WriteLine("SendInvoicesResponse");
                foreach (var r in response.response)
                {
                    Console.WriteLine($"{r.entitylineNumber}: status {r.statusCode}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
