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
                        aa = "1111",
                        issueDate = DateTime.Now,
                        invoiceType = InvoiceType.Item111 , // ΑΛΠ
                        currency = CurrencyType.EUR,
                        currencySpecified = true,
                    },
                    invoiceDetails = new InvoiceRowType[]
                    {
                        new InvoiceRowType
                        {
                            lineNumber = 1,
                            netValue = 5,
                            vatAmount = 5 * 0.24m,
                            vatCategory = 1 // 24%

                        },
                        new InvoiceRowType
                        {
                            lineNumber = 2,
                            netValue = 10,
                            vatAmount = 10 * 0.13m,
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
                        country = CountryType.GR
                    },
                    paymentMethods = new PaymentMethodDetailType[]
                    {
                        new PaymentMethodDetailType
                        {
                            amount = 15 + (5 * 0.24m + 10 * 0.13m),
                            type = 1,
                        }
                    }
                }
            };

            try
            {
                Console.WriteLine("SendInvoices ...");
                Console.WriteLine();
                var responseDoc = await client.SendInvoicesAsync(invoicesDoc);
                long? markToGet = null;
                foreach (var response in responseDoc.response)
                {
                    if (response.HasErrors())
                    {
                        var errors = response.GetErrors();
                        foreach (var error in errors)
                        {
                            Console.WriteLine($"{response.index}: status {response.statusCode}, error code {error.code}, error message: {error.message}");
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        markToGet = response.GetInvoiceMark();
                        Console.WriteLine($"{response.index}: status {response.statusCode}, Invoice Mark: {response.GetInvoiceMark()}, InvoiceUid: {response.GetInvoiceUid()}");
                        Console.WriteLine();
                    }
                }

                if (markToGet != null)
                {
                    Console.WriteLine("RequestInvoices ...");
                    Console.WriteLine();
                    var requestedDoc = await client.RequestTransmittedDocsAsync(markToGet.Value - 1);
                    if (requestedDoc.invoicesDoc?.invoice == null)
                    {
                        Console.WriteLine("No invoice found");
                    }
                    else
                    {
                        foreach (var invoice in requestedDoc.invoicesDoc.invoice)
                        {
                            Console.WriteLine($"Invoice: {invoice.mark}");
                        }
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
