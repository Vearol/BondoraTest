using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Data;
using Data.Interfaces;
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
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        private static Dictionary<int, Invoice> _userInvoices = new Dictionary<int, Invoice>();
        
        public CartController(ILogger<CartController> logger, IOrderItemRepository orderItemRepository, 
            IOrderRepository orderRepository, IUserRepository userRepository)
        {
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;

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
            var order = _orderRepository.GetBy(o => o.UserId == session.ID).FirstOrDefault();
            var orderItemsModel = new List<OrderItemModel>();
            if (order != null)
            {
                var orderItems = _orderItemRepository.GetBy(oi => oi.OrderId == order.Id)
                    .Include(oi => oi.Equipment).ToList();

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

            var order = _orderRepository.GetBy(o => o.UserId == session.ID).FirstOrDefault();
            if (order == null)
            {
                _logger.LogInformation($"Creating order placeholder for user ${session.ID}");
                order = new Order(DateTime.UtcNow, session.ID);
                _orderRepository.Create(order);
                _orderRepository.Save();
            }

            _orderItemRepository.Create(new OrderItem(DateTime.UtcNow, numberOfDays, order.Id, id));
            _orderItemRepository.Save();
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

            var orderToDelete = _orderItemRepository.GetById(id);

            _orderItemRepository.Delete(orderToDelete);
            _orderItemRepository.Save();

            _logger.LogInformation($"Removed order item {id} for user's {session.ID} cart");

            return RedirectToAction("Index");
            
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

            if (User.Identity.IsAuthenticated)
            {
                var user = _userRepository.GetBy(u => u.LoginNickname == User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var orderItems = _userInvoices[session.ID].GetOrderItems();

                    var loyaltyPoints = orderItems.Select(orderItem =>
                            new RentFee(orderItem.Equipment.Type, orderItem.RentDurationInDays))
                        .Select(rentFee => rentFee.CalculateLoyaltyPoints()).Sum();
                    user.ExtraPoints += loyaltyPoints;
                }
            }

            var orderToDelete = _orderRepository.GetBy(o => o.UserId == session.ID).FirstOrDefault();
            if (orderToDelete != null)
            {
                 _orderRepository.Delete(orderToDelete);
                _orderRepository.Save();
            }

            return RedirectToAction("ShowInvoice", "Cart");
        }
    }
}