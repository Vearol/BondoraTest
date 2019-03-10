using System.Collections.Generic;
using System.Text;
using Data.Models;
using Microsoft.AspNetCore.Html;
using PriceLogic.Rent;

namespace PriceLogic.Invoice
{
    public class Invoice
    {
        private List<OrderItem> _orderItems;
        private HtmlString _invoiceHtmlString;
        private float _priceSum;
        private int _orderId;

        public Invoice(int orderId, List<OrderItem> orderItems)
        {
            _orderId = orderId;
            _orderItems = orderItems;
        }

        private void GenerateInvoice()
        {
            var invoiceHtlmStringBuilder = new StringBuilder();

            invoiceHtlmStringBuilder.Append("<table>");

            for (var i = 0; i < _orderItems.Count; i++)
            {
                var currentItem = _orderItems[i];
                var rentFee = new RentFee(currentItem.Equipment.Type, currentItem.RentDurationInDays).CalculateFee();

                invoiceHtlmStringBuilder.Append("<tr>");
                invoiceHtlmStringBuilder.Append($"<td width=20px> {i + 1} </td>");
                invoiceHtlmStringBuilder.Append($"<td width=200px> {currentItem.Equipment.Name} </td>");
                invoiceHtlmStringBuilder.Append($"<td width=200px> {currentItem.Equipment.Type} </td>");
                invoiceHtlmStringBuilder.Append($"<td width=100px> {currentItem.RentDurationInDays} days </td>");
                invoiceHtlmStringBuilder.Append($"<td width=250px> {rentFee}$ </td>");
                invoiceHtlmStringBuilder.Append("</tr>");
                

                _priceSum += rentFee;
            }
            
            invoiceHtlmStringBuilder.Append($"<tr> <td width=100px>Total price: {_priceSum}$ </td> </tr>");

            invoiceHtlmStringBuilder.Append("</table>");

            _invoiceHtmlString = new HtmlString(invoiceHtlmStringBuilder.ToString());
        }

        public HtmlString GenerateInvoiceHtmlString()
        {
            if (_invoiceHtmlString == null) 
                GenerateInvoice();

            return _invoiceHtmlString;
        }

        public string GenerateInvoiceText()
        {
            var invoiceStringBuilder = new StringBuilder($"Invoice for order id: {_orderId}\n");

            for (var i = 0; i < _orderItems.Count; i++)
            {
                var currentItem = _orderItems[i];
                var rentFee = new RentFee(currentItem.Equipment.Type, currentItem.RentDurationInDays).CalculateFee();
                
                invoiceStringBuilder.Append($"{i + 1}.\t{currentItem.Equipment.Name}\t{currentItem.Equipment.Type}\t{currentItem.RentDurationInDays}\t{rentFee}$\n");
            }

            invoiceStringBuilder.Append($"Total price: {_priceSum}$");

            return invoiceStringBuilder.ToString();
        }

        public List<OrderItem> GetOrderItems()
        {
            return _orderItems;
        }

        public void AddToOrder(IEnumerable<OrderItem> orderItems)
        {
            _orderItems.AddRange(orderItems);
        }
    }
}
