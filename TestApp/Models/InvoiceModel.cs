using Microsoft.AspNetCore.Html;

namespace TestApp.Models
{
    public class InvoiceModel
    {
        public HtmlString Invoice { get; private set; }

        public InvoiceModel(HtmlString invoice)
        {
            Invoice = invoice;
        }
    }
}
