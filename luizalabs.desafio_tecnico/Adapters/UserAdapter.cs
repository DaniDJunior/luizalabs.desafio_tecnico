using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.User;

namespace luizalabs.desafio_tecnico.Adapters
{
    public class UserAdapter : IUserAdapter
    {
        private readonly IOrderAdapter OrderAdapter;

        public UserAdapter(IOrderAdapter orderAdapter)
        {
            OrderAdapter = orderAdapter;
        }

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

        public List<UserLegacyView> ToListViewLegacy(List<User> itens)
        {
            return itens.Select(item => ToViewLegacy(item)).ToList();
        }

        public UserLegacyView ToViewLegacy(User item)
        {
            UserLegacyView user = new UserLegacyView();

            user.user_id = item.legacy_user_id;
            user.name = item.name;

            user.orders = OrderAdapter.ToListLegacyView(item.orders.ToList());

            return user;
        }
    }
}
