using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MvcTest.Data;
using Newtonsoft.Json;
using TestApp.Data;
using TestApp.Data.Models;
using TestApp.Models;

namespace TestApp.Controllers
{
    public class CartController: Controller
    {
        private readonly StoreContext _context;
        private readonly ILogger _logger;
        
        public CartController(StoreContext context, ILogger<CartController> logger)
        {
            Debug.Assert(context != null);
            _context = context;
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

            var session = JsonConvert.DeserializeObject<Session>(sessionData);
            var order = _context.Orders.FirstOrDefault(o => o.UserID == session.ID);
            var items = new List<OrderItemModel>();
            if (order != null)
            {
                items = _context.OrderItems
                    .Include(oi => oi.Equipment)
                    .Where(oi => oi.OrderId == order.Id)
                    .Select(oi => new OrderItemModel(oi.Id, new EquipmentItemModel(oi.Equipment)))
                    .ToList();
            }
            else 
            {
                _logger.LogError($"No orders exist for user {session.ID}");
            }
            return View(items);
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
            var order = _context.Orders.FirstOrDefault(o => o.UserID == session.ID);
            if (order == null)
            {
                _logger.LogInformation($"Creating order placeholder for user ${session.ID}");
                order = new Order() 
                {
                    DateCreated = DateTime.UtcNow,
                    UserID = session.ID,
                    Completed = false
                };
                _context.Orders.Add(order);
                _context.SaveChanges();
            }

            _context.OrderItems.Add(
                new OrderItem()
                {
                    DateAdded = DateTime.UtcNow,
                    EquipmentId = id,
                    OrderId = order.Id
                });
            _context.SaveChanges();
            _logger.LogInformation($"Added order item {id} for user's {session.ID} cart");

            return RedirectToAction("Index");
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
            _context.OrderItems.Attach(orderItem);
            _context.OrderItems.Remove(orderItem);
            _context.SaveChanges();
            _logger.LogInformation($"Removed order item {id} for user's {session.ID} cart");

            return RedirectToAction("Index");
        }
    }
}