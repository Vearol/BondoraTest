using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PriceLogic.Invoice;
using TestApp.Models;

namespace TestApp.Controllers
{
    public class CartController: Controller
    {
        private readonly StoreContextFactory _contextFactory;
        private readonly ILogger _logger;

        private static Invoice _invoice;
        
        public CartController(ILogger<CartController> logger)
        {
            _contextFactory = new StoreContextFactory();

            Debug.Assert(logger != null);
            _logger = logger;
        }

        public IActionResult Index()
        {
            var sessionData = HttpContext.Session.GetString(Session.Key);
            if (string.IsNullOrEmpty(sessionData))
            {
                _logger.LogError("Attempt to list cart before accessing products list");
                return StatusCode(500);
            }

            using (var context = _contextFactory.CreateDbContext(null))
            {
                var session = JsonConvert.DeserializeObject<Session>(sessionData);
                var order = context.Orders.FirstOrDefault(o => o.UserID == session.ID);
                var orderItemsModel = new List<OrderItemModel>();
                if (order != null)
                {
                    var orderItems = context.OrderItems
                        .Include(oi => oi.Equipment)
                        .Where(oi => oi.OrderId == order.Id).ToArray();

                    _invoice = new Invoice(orderItems);

                    orderItemsModel = orderItems.Select(oi => new OrderItemModel(oi.Id, new EquipmentItemModel(oi.Equipment))).ToList();
                }
                else
                {
                    _logger.LogError($"No orders exist for user {session.ID}");
                }

                return View(orderItemsModel);
            }
        }

        [HttpPost]
        public IActionResult Order(int id)
        {
            var sessionData = HttpContext.Session.GetString(Session.Key);
            if (string.IsNullOrEmpty(sessionData))
            {
                _logger.LogError("Attempt to make an order before accessing products list");
                return StatusCode(500);
            }

            var session = JsonConvert.DeserializeObject<Session>(sessionData);

            using (var context = _contextFactory.CreateDbContext(null))
            {
                var order = context.Orders.FirstOrDefault(o => o.UserID == session.ID);
                if (order == null)
                {
                    _logger.LogInformation($"Creating order placeholder for user ${session.ID}");
                    order = new Order()
                    {
                        DateCreated = DateTime.UtcNow,
                        UserID = session.ID,
                        Completed = false
                    };
                    context.Orders.Add(order);
                    context.SaveChanges();
                }

                context.OrderItems.Add(
                    new OrderItem()
                    {
                        DateAdded = DateTime.UtcNow,
                        EquipmentId = id,
                        OrderId = order.Id
                    });
                context.SaveChanges();
                _logger.LogInformation($"Added order item {id} for user's {session.ID} cart");

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            var sessionData = HttpContext.Session.GetString(Session.Key);
            if (string.IsNullOrEmpty(sessionData))
            {
                _logger.LogError("Attempt to remove order item before accessing products list");
                return StatusCode(500);
            }

            var session = JsonConvert.DeserializeObject<Session>(sessionData);
            var orderItem = new OrderItem() { Id = id };

            using (var context = _contextFactory.CreateDbContext(null))
            {
                context.OrderItems.Attach(orderItem);
                context.OrderItems.Remove(orderItem);
                context.SaveChanges();

                _logger.LogInformation($"Removed order item {id} for user's {session.ID} cart");

                return RedirectToAction("Index");
            }
        }

        public IActionResult ShowInvoice()
        {
            return View(new InvoiceModel(_invoice.GetInvoiceHtmlString()));
        }
    }
}