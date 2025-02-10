using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Models.User;
using luizalabs.desafio_tecnico.test.LocalMock.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luizalabs.desafio_tecnico.test.Adapters
{
    [TestClass]
    public class UserAdapterTest
    {
        private UserAdapter UserAdapter;

        [TestInitialize]
        public void TestInitialize()
        {
            OrderAdapter orderAdapter = new OrderAdapter();

            UserAdapter = new UserAdapter(orderAdapter);
        }

        [TestMethod]
        public void ToView()
        {
            User user = UserSample.Sample();
            UserView userVies = UserAdapter.ToView(user);

            Assert.AreEqual(user.user_id, userVies.user_id);
            Assert.AreEqual(user.name, userVies.name);
        }

        [TestMethod]
        public void ToListView()
        {
            int sampleSize = 5;

            List<User> users = new List<User>();

            for (int i = 0; i < sampleSize; i++)
            {
                users.Add(UserSample.Sample());
            }

            List<UserView> usersVies = UserAdapter.ToListView(users);

            Assert.AreEqual(users.Count, usersVies.Count);

            for (int i = 0; i < sampleSize; i++)
            {
                Assert.AreEqual(users[i].user_id, usersVies[i].user_id);
                Assert.AreEqual(users[i].name, usersVies[i].name);
            }
        }

        [TestMethod]
        public void ToModelInsert()
        {
            UserInsert userInsert = UserSample.SampleInsert();
            User user = UserAdapter.ToModel(userInsert);

            Assert.AreEqual(user.user_id, Guid.Empty);
            Assert.AreEqual(user.name, userInsert.name);
        }

        [TestMethod]
        public void ToModelUpdate()
        {
            User userInit = UserSample.Sample();
            UserUpdate userUpdate = UserSample.SampleUpdate();
            User user = UserAdapter.ToModel(userUpdate, userInit);

            Assert.AreEqual(user.user_id, userInit.user_id);
            Assert.AreEqual(user.name, userUpdate.name);
        }
    }
}
