using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luizalabs.desafio_tecnico.test.LocalMock.Sample
{
    internal class UserSample
    {
        public static Models.User.User Sample()
        {
            Models.User.User user = new Models.User.User();

            user.user_id = Guid.NewGuid();
            user.name = ComunSample.RandomString(45);
            user.legacy_user_id = ComunSample.RandomInt();

            return user;
        }

        public static Models.User.UserInsert SampleInsert()
        {
            Models.User.UserInsert user = new Models.User.UserInsert();

            user.name = ComunSample.RandomString(45);

            return user;
        }

        public static Models.User.UserUpdate SampleUpdate()
        {
            Models.User.UserUpdate user = new Models.User.UserUpdate();

            user.name = ComunSample.RandomString(45);

            return user;
        }
    }
}
