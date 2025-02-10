using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Controllers;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Logics;
using luizalabs.desafio_tecnico.test.Communications;
using luizalabs.desafio_tecnico.test.LocalMock.Sample;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    public class LegacyControllerTest
    {
        private ILegacyData LegacyData;
        private IUserData UserData;
        private IOrderData OrderData;
        private ILegacyLogic LegacyLogic;
        private ILegacyAdapter LegacyAdapter;
        private IConfiguration Configuration;
        private ILogger<LegacyController> Logger;

        [TestInitialize]
        public void TestInitialize()
        {
            var mockControles = new Mock<ILogger<LegacyController>>();
            ILogger<LegacyController> Logger = mockControles.Object;

            LegacyData = new LocalMock.Data.LegacyData();
            UserData = new LocalMock.Data.UserData();
            OrderData = new LocalMock.Data.OrderData();

            var mockLogic = new Mock<ILogger<LegacyLogic>>();
            ILogger<LegacyLogic> LoggerLogic = mockLogic.Object;

            var inMemorySettings = new Dictionary<string, string> {
                {"Legacy:Bkp_Patch", "Bkp/"},
            };

            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            IKafkaCommunication kafkaCommunication = new KafkaCommunication();

            LegacyLogic = new LegacyLogic(LoggerLogic, Configuration, kafkaCommunication, LegacyData, UserData, OrderData);

            LegacyAdapter = new LegacyAdapter();
        }

        [TestMethod]
        public async Task Get()
        {
            LegacyController legacyController = new LegacyController(Logger, Configuration, LegacyAdapter, LegacyLogic, LegacyData);

            var result = await legacyController.Get(null);

            var okResult = result as ObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [TestMethod]
        public async Task GetID()
        {
            var request = LegacyRequestSample.Sample();
            await LegacyData.SaveAsync(request);

            LegacyController legacyController = new LegacyController(Logger, Configuration, LegacyAdapter, LegacyLogic, LegacyData);

            var result = await legacyController.GetId(request.request_id);

            var okResult = result as ObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.AreEqual(request.request_id, ((Models.Legacy.LegacyView)okResult.Value).request_id);
        }

        [TestMethod]
        public async Task GetIDNotFound()
        {
            LegacyController legacyController = new LegacyController(Logger, Configuration, LegacyAdapter, LegacyLogic, LegacyData);

            var result = await legacyController.GetId(Guid.NewGuid());

            var okResult = result as StatusCodeResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.NotFound, okResult.StatusCode);
        }
    }
}
