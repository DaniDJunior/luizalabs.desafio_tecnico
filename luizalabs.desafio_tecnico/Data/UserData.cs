using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.User;
using Microsoft.EntityFrameworkCore;

namespace luizalabs.desafio_tecnico.Data
{
    public class UserData : IUserData
    {
        private readonly ILogger<UserData> Logger;
        private readonly IServiceProvider ServiceProvider;

        public UserData(ILogger<UserData> logger, IServiceProvider serviceProvider)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
        }

        public async Task<List<Models.User.User>> GetListAsync()
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                List<Models.User.User> users = await dataContext.Users.Include(u => u.orders).ThenInclude(o => o.products).ToListAsync();
                return users;
            }
        }

        public async Task<List<Models.User.User>> GetListByRequestIdAsync(Guid id, string? user_name, DateTime? datemin, DateTime? datemax)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                List<int> usersId = await dataContext.RequestsLines.Where(r => r.request_id == id && (user_name == null || r.user_name.Contains(user_name)) && (datemin == null || r.order_date >= datemin.Value) && (datemax == null || r.order_date <= datemax.Value))
                    .Select(x => x.user_id).ToListAsync();
                List<Models.User.User> users = await dataContext.Users.Include(u => u.orders).ThenInclude(o => o.products).Where(user => user.legacy_user_id != null && usersId.Contains(user.legacy_user_id.Value)).ToListAsync();

                return users;
            }
        }

        public async Task<Models.User.User?> GetByIdAsync(Guid id)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                Models.User.User? user = await dataContext.Users.Include(u => u.orders).ThenInclude(o => o.products).FirstOrDefaultAsync(user => user.user_id == id);
                return user;
            }
        }

        public async Task<Models.User.User> SaveAsync(Models.User.User user)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                if (user.user_id == Guid.Empty)
                {
                    user.user_id = Guid.NewGuid();
                    dataContext.Entry(user).State = EntityState.Added;
                }
                else
                {
                    dataContext.Entry(user).State = EntityState.Modified;
                }
                await dataContext.SaveChangesAsync();
                return user;
            }
        }

        public async Task<bool> DeleteAsync(Models.User.User user)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                dataContext.Users.Remove(user);
                await dataContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<User?> GetByIdLegacyAsync(int id)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                Models.User.User? user = await dataContext.Users.Include(u => u.orders).ThenInclude(o => o.products).FirstOrDefaultAsync(user => user.legacy_user_id == id);
                return user;
            }
        }
    }
}
