using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.User;

namespace luizalabs.desafio_tecnico.Adapters
{
    public class UserAdapter : IUserAdapter
    {
        public List<UserView> ToListView(List<User> itens)
        {
            return itens.Select(item => ToView(item)).ToList();
        }

        public User ToModel(UserInsert item)
        {
            User user = new User();
            user.user_id = Guid.Empty;
            user.name = item.name;

            return user;
        }

        public User ToModel(UserUpdate item, User user)
        {
            user.name = item.name;

            return user;
        }

        public UserView ToView(User item)
        {
            UserView user = new UserView();

            user.user_id = item.user_id;
            user.name = item.name;

            return user;
        }
    }
}
