using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcTest.Data;
using Newtonsoft.Json;
using TestApp.Data;
using TestApp.Models;

namespace TestApp.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly StoreContext _context;
        private readonly ILogger _logger;

        public EquipmentController(StoreContext context, ILogger<EquipmentController> logger)
        {
            Debug.Assert(context != null);
            _context = context;
            Debug.Assert(logger != null);
            _logger = logger;
        }

        public IActionResult List()
        {
            Session session = null;
            var value = HttpContext.Session.GetString(Session.Key);
            if (string.IsNullOrEmpty(value))
            {
                _logger.LogInformation("Handling new user's listing");
                session = Session.New();
                var serialisedDate = JsonConvert.SerializeObject(session);
                HttpContext.Session.SetString(Session.Key, serialisedDate);
            }
            else
            {
                session = JsonConvert.DeserializeObject<Session>(value);
                _logger.LogInformation($"Handling user {session.ID} listing");
            }

            var equipmentItemModelList = _context.EquipmentItems.Select(x => new EquipmentItemModel(x.Id, x.Name, x.Type)).ToArray();

            return View(equipmentItemModelList);
        }

        public IActionResult Details(int id)
        {
            _logger.LogDebug($"Requested details about product {id}");


            var equipmentItem = _context.EquipmentItems.FirstOrDefault(x => x.Id == id);

            if (equipmentItem == null)
            {
                throw new Exception($"Cannot find equipment item with id={id}");
            }

            var equipmentItemModel = new EquipmentItemModel(equipmentItem);

            return View(equipmentItemModel);
        }
    }
}