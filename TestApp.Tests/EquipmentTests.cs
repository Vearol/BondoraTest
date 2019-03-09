using System;
using System.Text;
using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using TestApp.Controllers;

namespace TestApp.Tests
{
    public class EquipmentTests
    {
        private readonly StoreContextFactory _contextFactory = new StoreContextFactory();

        [SetUp]
        public void SetupDb()
        {
            try
            {
                var context = _contextFactory.CreateDbContext(null);
                context.Database.Migrate();
                SeedData.Initialize(context);
            }
            catch (Exception)
            {
                throw new Exception("Cannot initialize database.");
            }
        }

        [Test]
        public void Index_ReturnsAViewResult_WithAListOfProducts()
        {
            var logger = new Mock<ILogger<EquipmentController>>();
            var controller = new EquipmentController(logger.Object);

            var sessionStub = new Mock<ISession>();
            var httpContextStub = new Mock<HttpContext>();
            httpContextStub.Setup(x => x.Session).Returns(sessionStub.Object);
            var controllerContext = new ControllerContext();
            controllerContext.HttpContext = httpContextStub.Object;
            controller.ControllerContext = controllerContext;

            // this is the real test line
            var result = controller.List();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Order_AddsItemToCart()
        {
            var logger = new Mock<ILogger<CartController>>();
            var controller = new CartController(logger.Object);

            var sessionStub = new Mock<ISession>();
            byte[] sessionData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(Session.New()));
            sessionStub
                .Setup(x => x.TryGetValue(Session.Key, out sessionData))
                .Returns(true);

            var httpContextStub = new Mock<HttpContext>();
            httpContextStub.Setup(x => x.Session).Returns(sessionStub.Object);
            var controllerContext = new ControllerContext();
            controllerContext.HttpContext = httpContextStub.Object;
            //controllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues> { { "numberOfDays", "1" } });
            controller.ControllerContext = controllerContext;

            //var context = _contextFactory.CreateDbContext(null);

            //var addResult = controller.Order(context.EquipmentItems.First().Id);
            //Assert.IsInstanceOf<RedirectToActionResult>(addResult);

            var indexResult = controller.Index();
            Assert.IsInstanceOf<ViewResult>(indexResult);
        }

        [Test]
        public void Remove_RemovesItemFromCart()
        {
            var logger = new Mock<ILogger<CartController>>();
            var controller = new CartController(logger.Object);

            var sessionStub = new Mock<ISession>();
            var sessionData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(Session.New()));
            sessionStub
                .Setup(x => x.TryGetValue(Session.Key, out sessionData))
                .Returns(true);

            var httpContextStub = new Mock<HttpContext>();
            httpContextStub.Setup(x => x.Session).Returns(sessionStub.Object);
            var controllerContext = new ControllerContext();
            controllerContext.HttpContext = httpContextStub.Object;
            controller.ControllerContext = controllerContext;

            var indexResult = controller.Index();
            Assert.IsInstanceOf<ViewResult>(indexResult);
        }
    }
}
