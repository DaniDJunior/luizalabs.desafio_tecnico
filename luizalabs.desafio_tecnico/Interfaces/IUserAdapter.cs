using luizalabs.desafio_tecnico.Models.User;

namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface IUserAdapter
    {
        public List<UserView> ToListView(List<User> itens);

        public List<UserLegacyView> ToListViewLegacy(List<User> itens);

        public UserLegacyView ToViewLegacy(User item);

        public UserView ToView(User item);

        public User ToModel(UserInsert item);

        public User ToModel(UserUpdate item, User user);
    }
}
