using System.Text;
using Data.Models;
using Microsoft.AspNetCore.Html;
using PriceLogic.Rent;

namespace PriceLogic.Invoice
{
    public class Invoice
    {
        private OrderItem[] _orderItems;

        private HtmlString _invoiceHtmlString;

        public Invoice(OrderItem[] orderItems)
        {
            _orderItems = orderItems;
        }

        private void GenerateInvoice()
        {
            var invoiceHtlmStringBuilder = new StringBuilder();
            var priceSum = 0f;

            invoiceHtlmStringBuilder.Append("<table>");

            for (var i = 0; i < _orderItems.Length; i++)
            {
                var currentItem = _orderItems[i];
                var rentFee = new RentFee(currentItem.Equipment.Type, 1).CalculateFee();

                invoiceHtlmStringBuilder.Append("<tr>");
                invoiceHtlmStringBuilder.Append($"<td width=20px> {i} </td>");
                invoiceHtlmStringBuilder.Append($"<td width=200px> {currentItem.Equipment.Name} </td>");
                invoiceHtlmStringBuilder.Append($"<td width=200px> {currentItem.Equipment.Type} </td>");
                invoiceHtlmStringBuilder.Append($"<td width=250px> {rentFee}$ </td>");
                invoiceHtlmStringBuilder.Append("</tr>");
                

                priceSum += rentFee;
            }
            
            invoiceHtlmStringBuilder.Append($"<tr> <td width=100px>Total price: {priceSum}$ </td> </tr>");

            invoiceHtlmStringBuilder.Append("</table>");

            _invoiceHtmlString = new HtmlString(invoiceHtlmStringBuilder.ToString());
        }

        public HtmlString GetInvoiceHtmlString()
        {
            if (_invoiceHtmlString == null) 
                GenerateInvoice();

            return _invoiceHtmlString;
        }
    }
}
