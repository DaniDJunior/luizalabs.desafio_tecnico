using luizalabs.desafio_tecnico.Data;

namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface IUserData
    {
        public Task<List<Models.User.User>> GetListAsync();

        public Task<List<Models.User.User>> GetListByRequestIdAsync(Guid id, string? user_name, DateTime? datemin, DateTime? datemax);

        public Task<Models.User.User?> GetByIdAsync(Guid id);

        public Task<Models.User.User?> GetByIdLegacyAsync(int id);

        public Task<Models.User.User> SaveAsync(Models.User.User user);

        public Task<bool> DeleteAsync(Models.User.User user);
    }
}
