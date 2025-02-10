using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Controllers;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.test.LocalMock.Sample;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace luizalabs.desafio_tecnico.test.Controllers
{
    [TestClass]
    public class OrderControllerTest
    {
        private IOrderData OrderData;
        private ILogger<OrderController> Logger;
        private IOrderAdapter OrderAdapter;

        [TestInitialize]
        public void TestInitialize()
        {
            var mock = new Mock<ILogger<OrderController>>();
            ILogger<OrderController> Logger = mock.Object;

            OrderAdapter orderAdapter = new OrderAdapter();
            UserAdapter userAdapter = new UserAdapter(orderAdapter);

            OrderAdapter = new OrderAdapter();
            OrderData = new LocalMock.Data.OrderData();
        }

        [TestMethod]
        public async Task Get()
        {
            OrderController orderController = new OrderController(Logger, OrderAdapter, OrderData);

            var result = await orderController.Get();

            var okResult = result as ObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [TestMethod]
        public async Task GetID()
        {
            var order = OrderSample.Sample();
            await OrderData.SaveAsync(order);

            OrderController orderController = new OrderController(Logger, OrderAdapter, OrderData);

            var result = await orderController.GetId(order.order_id);

            var okResult = result as ObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.AreEqual(order.order_id, ((Models.Order.OrderView)okResult.Value).order_id);
        }

        [TestMethod]
        public async Task GetIDNotFound()
        {
            OrderController orderController = new OrderController(Logger, OrderAdapter, OrderData);

            var result = await orderController.GetId(Guid.NewGuid());

            var okResult = result as StatusCodeResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.NotFound, okResult.StatusCode);
        }

        [TestMethod]
        public async Task Post()
        {

            Models.Order.OrderInsert orderInsert = OrderSample.SampleInsert();
            OrderController orderController = new OrderController(Logger, OrderAdapter, OrderData);
            var result = await orderController.Post(orderInsert);

            var okResult = result as ObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(201, okResult.StatusCode);
            Assert.AreNotEqual(Guid.Empty, ((Models.Order.OrderView)okResult.Value).order_id);
        }

        [TestMethod]
        public async Task Put()
        {
            Models.Order.Order order = OrderSample.Sample();

            await OrderData.SaveAsync(order);

            Models.Order.OrderUpdate orderUpdate = OrderSample.SampleUpdate();

            OrderController orderController = new OrderController(Logger, OrderAdapter, OrderData);
            var result = await orderController.Put(orderUpdate, order.order_id);

            var okResult = result as ObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(order.order_id, (okResult.Value as Models.Order.OrderView).order_id);
            Assert.AreEqual(order.date, (okResult.Value as Models.Order.OrderView).date);
        }

        [TestMethod]
        public async Task Delete()
        {
            Models.Order.Order user = OrderSample.Sample();

            await OrderData.SaveAsync(user);

            OrderController orderController = new OrderController(Logger, OrderAdapter, OrderData);
            var result = await orderController.Delete(user.order_id);

            var okResult = result as StatusCodeResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual((await OrderData.GetListAsync()).Count, 0);
        }

        [TestMethod]
        public async Task DeleteNotFound()
        {
            OrderController orderController = new OrderController(Logger, OrderAdapter, OrderData);
            var result = await orderController.Delete(Guid.NewGuid());

            var okResult = result as StatusCodeResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(404, okResult.StatusCode);
        }
    }
}
