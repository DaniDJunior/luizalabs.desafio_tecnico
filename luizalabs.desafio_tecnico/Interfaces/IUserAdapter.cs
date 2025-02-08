namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface IUserAdapter
    {
        public List<Models.User.UserView> ToListView(List<Models.User.User> itens);

        public Models.User.UserView ToView(Models.User.User item);

        public Models.User.User ToModel(Models.User.UserInsert item);

        public Models.User.User ToModel(Models.User.UserUpdate item, Models.User.User user);
    }
}
