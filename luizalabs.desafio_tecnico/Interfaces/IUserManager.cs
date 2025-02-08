namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface IUserManager
    {
        public Task<List<Models.User.User>> GetListAsync(bool load);

        public Task<Models.User.User?> GetId(Guid id, bool load);

        public Task<Models.User.User> Load(Models.User.User user);

        public Task<Models.User.User> Save(Models.User.User user);

        public Task<bool> Delete(Models.User.User user);
    }
}
