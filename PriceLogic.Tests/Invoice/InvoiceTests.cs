using System;
using System.Collections.Generic;
using Data.Models;
using NUnit.Framework;

namespace PriceLogic.Tests.Invoice
{
    public class InvoiceTests
    {
        [Test]
        public void InvoiceHtmlStringWithEmptyOrder()
        {
            var testInvoice = new PriceLogic.Invoice.Invoice(123, new List<OrderItem>());

            var expected = "<table><tr> <td width=100px>Total price: 0$ </td> </tr></table>";

            Assert.AreEqual(testInvoice.GenerateInvoiceHtmlString().Value, expected);
        }

        [Test]
        public void InvoiceTextWithEmptyOrder()
        {
            var testInvoice = new PriceLogic.Invoice.Invoice(123, new List<OrderItem>());

            var expected = "Invoice for order id: 123\nTotal price: 0$";

            Assert.AreEqual(testInvoice.GenerateInvoiceText(), expected);
        }

        [Test]
        public void InvoiceTextWithNullOrder()
        {
            var testInvoice = new PriceLogic.Invoice.Invoice(123, null);

            Assert.Throws<NullReferenceException>(() => testInvoice.GenerateInvoiceText());
        }

        [Test]
        public void InvoiceHtmlStringWithNullOrder()
        {
            var testInvoice = new PriceLogic.Invoice.Invoice(123, null);

            Assert.Throws<NullReferenceException>(() => testInvoice.GenerateInvoiceHtmlString());
        }
    }
}
