using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PriceLogic.Invoice;
using PriceLogic.Rent;
using TestApp.Models;

namespace TestApp.Controllers
{
    public class CartController: Controller
    {
        private readonly StoreContextFactory _contextFactory;
        private readonly ILogger _logger;

        private static Dictionary<int, Invoice> _userInvoices = new Dictionary<int, Invoice>();
        
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
                var order = context.Orders.FirstOrDefault(o => o.UserId == session.ID);
                var orderItemsModel = new List<OrderItemModel>();
                if (order != null)
                {
                    var orderItems = context.OrderItems
                        .Include(oi => oi.Equipment)
                        .Where(oi => oi.OrderId == order.Id).ToList();

                    _userInvoices[session.ID] = new Invoice(order.Id, orderItems);

                    orderItemsModel = orderItems.Select(oi => new OrderItemModel(oi.Id, oi.RentDurationInDays, 
                        new EquipmentItemModel(oi.Equipment))).ToList();
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

            var numberOfDays = int.Parse(Request.Form["numberOfDays"]);

            var session = JsonConvert.DeserializeObject<Session>(sessionData);

            using (var context = _contextFactory.CreateDbContext(null))
            {
                var order = context.Orders.FirstOrDefault(o => o.UserId == session.ID);
                if (order == null)
                {
                    _logger.LogInformation($"Creating order placeholder for user ${session.ID}");
                    order = new Order(DateTime.UtcNow, session.ID);
                    context.Orders.Add(order);
                    context.SaveChanges();
                }

                context.OrderItems.Add(new OrderItem(DateTime.UtcNow, numberOfDays, order.Id, id));
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
            
            using (var context = _contextFactory.CreateDbContext(null))
            {
                var orderToDelete = context.OrderItems.First(oi => oi.Id == id);

                context.OrderItems.Remove(orderToDelete);
                context.SaveChanges();

                _logger.LogInformation($"Removed order item {id} for user's {session.ID} cart");

                return RedirectToAction("Index");
            }
        }

        public IActionResult ShowInvoice()
        {
            var sessionData = HttpContext.Session.GetString(Session.Key);
            if (string.IsNullOrEmpty(sessionData))
            {
                _logger.LogError("Attempt to make an order before accessing products list");
                return StatusCode(500);
            }
            var session = JsonConvert.DeserializeObject<Session>(sessionData);

            return View(new InvoiceModel(_userInvoices[session.ID].GenerateInvoiceHtmlString()));
        }

        public IActionResult DownloadInvoice()
        {
            var sessionData = HttpContext.Session.GetString(Session.Key);
            if (string.IsNullOrEmpty(sessionData))
            {
                _logger.LogError("Attempt to make an order before accessing products list");
                return StatusCode(500);
            }
            var session = JsonConvert.DeserializeObject<Session>(sessionData);
            return File(Encoding.UTF8.GetBytes(_userInvoices[session.ID].GenerateInvoiceText()), "text/plain", "invoice.txt"); ;
        }

        public IActionResult Checkout()
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
                if (User.Identity.IsAuthenticated)
                {
                    var user = context.Users.FirstOrDefault(u => u.LoginNickname == User.Identity.Name);
                    if (user != null)
                    {
                        var orderItems = _userInvoices[session.ID].GetOrderItems();

                        var loyaltyPoints = orderItems.Select(orderItem => new RentFee(orderItem.Equipment.Type, orderItem.RentDurationInDays))
                                                      .Select(rentFee => rentFee.CalculateLoyaltyPoints()).Sum();
                        user.ExtraPoints += loyaltyPoints;
                    }
                }

                var orderToDelete = context.Orders.FirstOrDefault(o => o.UserId == session.ID);
                if (orderToDelete != null)
                {
                    context.Orders.Remove(orderToDelete);

                    context.SaveChanges();
                }
            }

            return RedirectToAction("ShowInvoice", "Cart");
        }
    }
}