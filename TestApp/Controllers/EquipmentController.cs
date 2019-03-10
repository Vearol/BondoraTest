using System;
using System.Diagnostics;
using System.Linq;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TestApp.Models;

namespace TestApp.Controllers
{
    [Authorize]
    public class EquipmentController : Controller
    {
        private readonly StoreContextFactory _contextFactory;
        private readonly ILogger _logger;

        public EquipmentController(ILogger<EquipmentController> logger)
        {
            _contextFactory = new StoreContextFactory();

            Debug.Assert(logger != null);
            _logger = logger;
        }

        [AllowAnonymous]
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

            using (var context = _contextFactory.CreateDbContext(null))
            {
                var equipmentItemModelList = context.EquipmentItems.Select(x => new EquipmentItemModel(x.Id, x.Name, x.Type)).ToArray();

                return View(equipmentItemModelList);
            }
        }

        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            _logger.LogDebug($"Requested details about product {id}");

            using (var context = _contextFactory.CreateDbContext(null))
            {
                var equipmentItem = context.EquipmentItems.FirstOrDefault(x => x.Id == id);

                if (equipmentItem == null)
                {
                    throw new Exception($"Cannot find equipment item with id={id}");
                }

                var equipmentItemModel = new EquipmentItemModel(equipmentItem);

                return View(equipmentItemModel);
            }
        }
    }
}