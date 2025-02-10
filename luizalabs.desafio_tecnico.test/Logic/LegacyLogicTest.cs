using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Controllers;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Logics;
using luizalabs.desafio_tecnico.Models.Legacy;
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

namespace luizalabs.desafio_tecnico.test.Logic
{
    [TestClass]
    public class LegacyLogicTest
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
        }

        [TestMethod]
        public async Task ProcessLineAsync()
        {
            LegacyRequest request = LegacyRequestSample.Sample();

            await LegacyData.SaveAsync(request);

            string line = "0000000017                              Ethan Langworth00000001690000000001      865.1820210409";

            await LegacyLogic.ProcessLineAsync(line, 0, request.request_id);

            Assert.AreEqual(request.lines.Count, 1);
            Assert.AreEqual(request.errors.Count, 0);

        }

        [TestMethod]
        public async Task ProcessLineErrorAsync()
        {
            LegacyRequest request = LegacyRequestSample.Sample();

            await LegacyData.SaveAsync(request);

            string line = "000000X017                              Ethan Langworth00000001690000000001      865.1820210409";

            await LegacyLogic.ProcessLineAsync(line, 0, request.request_id);

            Assert.AreEqual(request.lines.Count, 0);
            Assert.AreEqual(request.errors.Count, 1);

        }

        [TestMethod]
        public async Task ProcessDataAsync()
        {
            LegacyRequest request = LegacyRequestSample.Sample();

            request = LegacyRequestSample.SampleAddLine(request);

            await LegacyData.SaveAsync(request);

            await LegacyLogic.ProcessDataAsync(request.lines.First().request_line_id);

            var users = await UserData.GetListAsync();
            var orders = await OrderData.GetListAsync();

            Assert.IsTrue(users.Count > 0);
            Assert.IsTrue(orders.Count > 0);
            Assert.IsTrue(orders.First().products.Count > 0);

        }
    }
}
