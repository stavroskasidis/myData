using myData.Client.Schema;
using System.Threading.Tasks;

namespace myData.Client
{
    public interface ImyDataClient
    {
        ResponseDoc CancelInvoice(string mark);
        Task<ResponseDoc> CancelInvoiceAsync(string mark);

        RequestedDoc RequestDocs(long? mark = null);
        Task<RequestedDoc> RequestDocsAsync(long? mark = null);


        RequestedDoc RequestTransmittedDocs(long mark);
        Task<RequestedDoc> RequestTransmittedDocsAsync(long mark);


        ResponseDoc SendExpensesClassification(ExpensesClassificationsDoc expensesClassificationsDoc);
        Task<ResponseDoc> SendExpensesClassificationAsync(ExpensesClassificationsDoc expensesClassificationsDoc);

        ResponseDoc SendIncomeClassification(IncomeClassificationsDoc incomeClassificationsDoc);
        Task<ResponseDoc> SendIncomeClassificationAsync(IncomeClassificationsDoc incomeClassificationsDoc);

        ResponseDoc SendInvoices(InvoicesDoc invoicesDoc);
        Task<ResponseDoc> SendInvoicesAsync(InvoicesDoc invoicesDoc);
    }
}