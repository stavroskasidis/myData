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
            var client = new myDataClient("takistoleizer", "8c9917040eda444b8d600914d54274a2", "https://mydata-dev.azure-api.net", httpClient);

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
                        invoiceType = InvoiceType.Item111 , // ΑΛΠ
                        currency = CurrencyType.EUR,
                        currencySpecified = true
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
                        totalGrossValue = 15 + (5 * 0.24m + 10 * 0.13m),
                    },
                    issuer = new PartyType
                    {
                        vatNumber = "123456789",
                        country = CountryType.GR,
                        address = new AddressType
                        {
                            street = "str",
                            city = "test",
                            number = "33",
                            postalCode = "33445"
                        }
                    }
                }
            };

            try
            {
                Console.WriteLine("SendInvoices ...");
                var response = await client.SendInvoicesAsync(invoicesDoc);
                foreach (var r in response.response)
                {
                    Console.WriteLine($"{r.entitylineNumber}: status {r.statusCode}, data: {string.Join(" ",r.Items.Select(x=> x.ToString()))}");
                }


                Console.WriteLine("RequestInvoices ...");
                var response2 = await client.RequestInvoicesAsync("0");
                if (response2.invoicesDoc.invoice == null)
                {
                    Console.WriteLine("No invoice found");
                }
                else
                {
                    foreach (var invoice in response2.invoicesDoc.invoice)
                    {
                        Console.WriteLine($"Invoice: {invoice.mark}");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadKey();
        }
    }
}
