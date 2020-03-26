using myData.Client.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myData.Client
{
    public static class ExtensionMethods
    {

        public static bool HasErrors(this ResponseType responseType)
        {
            return responseType.ItemsElementName.Any(x => x == ItemsChoiceType.errors);
        }

        public static IEnumerable<ErrorType> GetErrors(this ResponseType responseType)
        {
            if (responseType.Items == null || !responseType.ItemsElementName.Any(x=> x== ItemsChoiceType.errors)) return new List<ErrorType>();
            return responseType.Items.OfType<ResponseTypeErrors>().SelectMany(x=> x.error);
        }

        public static long? GetClassificationMark(this ResponseType responseType)
        {
            if (responseType.Items == null || !responseType.ItemsElementName.Any(x => x == ItemsChoiceType.classificationMark)) return null;
            var indexes = responseType.ItemsElementName.GetIndexesOfValue(ItemsChoiceType.classificationMark);
            return (long?)responseType.Items.ElementsAt(indexes).FirstOrDefault();
        }

        public static long? GetInvoiceMark(this ResponseType responseType)
        {
            if (responseType.Items == null || !responseType.ItemsElementName.Any(x => x == ItemsChoiceType.invoiceMark)) return null;
            var indexes = responseType.ItemsElementName.GetIndexesOfValue(ItemsChoiceType.invoiceMark);
            return (long?)responseType.Items.ElementsAt(indexes).FirstOrDefault();
        }

        public static string GetInvoiceUid(this ResponseType responseType)
        {
            if (responseType.Items == null || !responseType.ItemsElementName.Any(x => x == ItemsChoiceType.invoiceUid)) return null;
            var indexes = responseType.ItemsElementName.GetIndexesOfValue(ItemsChoiceType.invoiceUid);
            return (string)responseType.Items.ElementsAt(indexes).FirstOrDefault();
        }


        private static IEnumerable<int> GetIndexesOfValue<T>(this T[] array, T val)
        {
            return array.Select((value, index) => new { index, value })
                                                       .Where(x => EqualityComparer<T>.Default.Equals(x.value,val))
                                                       .Select(x => x.index);
        }

        private static IEnumerable<T> ElementsAt<T>(this IEnumerable<T> list,IEnumerable<int> indexes)
        {
            return list.Select((value, index) => new { index, value })
                       .Where(x => indexes.Contains(x.index))
                       .Select(x=> x.value);
        }
    }
}
