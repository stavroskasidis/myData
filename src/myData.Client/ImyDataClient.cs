using myData.Client.Schema;
using System.Threading.Tasks;

namespace myData.Client
{
    public interface ImyDataClient
    {
        RequestedInvoicesDoc RequestInvoices(string mark, string nextPartitionKey = null, string nextRowKey = null);
        Task<RequestedInvoicesDoc> RequestInvoicesAsync(string mark, string nextPartitionKey = null, string nextRowKey = null);


        RequestedInvoicesDoc RequestIssuerInvoices(string mark, string nextPartitionKey = null, string nextRowKey = null);
        Task<RequestedInvoicesDoc> RequestIssuerInvoicesAsync(string mark, string nextPartitionKey = null, string nextRowKey = null);


        ResponseDoc SendExpensesClassification(ExpensesClassificationsDoc expensesClassificationsDoc);
        Task<ResponseDoc> SendExpensesClassificationAsync(ExpensesClassificationsDoc expensesClassificationsDoc);

        ResponseDoc SendIncomeClassification(IncomeClassificationsDoc incomeClassificationsDoc);
        Task<ResponseDoc> SendIncomeClassificationAsync(IncomeClassificationsDoc incomeClassificationsDoc);

        ResponseDoc SendInvoices(InvoicesDoc invoicesDoc);
        Task<ResponseDoc> SendInvoicesAsync(InvoicesDoc invoicesDoc);
    }
}