using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Controllers;
using luizalabs.desafio_tecnico.Data;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.User;
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
    public class UserControllerTest
    {
        private IUserData UserData;
        private ILogger<UserController> Logger;
        private IUserAdapter UserAdapter;

        [TestInitialize]
        public void TestInitialize()
        {
            var mock = new Mock<ILogger<UserController>>();
            ILogger<UserController> Logger = mock.Object;

            OrderAdapter orderAdapter = new OrderAdapter();

            UserAdapter = new UserAdapter(orderAdapter);
            UserData = new LocalMock.Data.UserData();
        }

        [TestMethod]
        public async Task Get()
        {
            UserController userController = new UserController(Logger, UserAdapter, UserData);

            var result = await userController.Get();

            var okResult = result as ObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [TestMethod]
        public async Task GetID()
        {
            var user = UserSample.Sample();
            await UserData.SaveAsync(user);

            UserController userController = new UserController(Logger, UserAdapter, UserData);

            var result = await userController.GetId(user.user_id);

            var okResult = result as ObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.AreEqual(user.user_id, ((Models.User.UserView)okResult.Value).user_id);
        }

        [TestMethod]
        public async Task GetIDNotFound()
        {
            UserController userController = new UserController(Logger, UserAdapter, UserData);

            var result = await userController.GetId(Guid.NewGuid());

            var okResult = result as StatusCodeResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.NotFound, okResult.StatusCode);
        }

        [TestMethod]
        public async Task Post()
        {

            Models.User.UserInsert userInsert = UserSample.SampleInsert();
            UserController userController = new UserController(Logger, UserAdapter, UserData);
            var result = await userController.Post(userInsert);

            var okResult = result as ObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(201, okResult.StatusCode);
            Assert.AreNotEqual(Guid.Empty, ((Models.User.UserView)okResult.Value).user_id);
        }

        [TestMethod]
        public async Task Put()
        {
            Models.User.User user = UserSample.Sample();

            await UserData.SaveAsync(user);

            Models.User.UserUpdate userUpdate = UserSample.SampleUpdate();

            UserController userController = new UserController(Logger, UserAdapter, UserData);
            var result = await userController.Put(userUpdate, user.user_id);

            var okResult = result as ObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(user.user_id, (okResult.Value as Models.User.UserView).user_id);
            Assert.AreEqual(user.name, (okResult.Value as Models.User.UserView).name);
        }

        [TestMethod]
        public async Task Delete()
        {
            Models.User.User user = UserSample.Sample();

            await UserData.SaveAsync(user);

            UserController userController = new UserController(Logger, UserAdapter, UserData);
            var result = await userController.Delete(user.user_id);

            var okResult = result as StatusCodeResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual((await UserData.GetListAsync()).Count, 0);
        }

        [TestMethod]
        public async Task DeleteNotFound()
        {
            UserController userController = new UserController(Logger, UserAdapter, UserData);
            var result = await userController.Delete(Guid.NewGuid());

            var okResult = result as StatusCodeResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(404, okResult.StatusCode);
        }
    }
}
