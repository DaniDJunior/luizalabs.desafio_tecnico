using luizalabs.desafio_tecnico.Data;
using luizalabs.desafio_tecnico.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace luizalabs.desafio_tecnico.Managers
{
    public class UserManager : IUserManager
    {
        private readonly ILogger<UserManager> Logger;
        private readonly DataContext DataContext;
        private readonly IOrderManager OrderManager;

        public UserManager(ILogger<UserManager> logger, DataContext dataContext, IOrderManager orderManager)
        {
            Logger = logger;
            DataContext = dataContext;
            OrderManager = orderManager;
        }

        public async Task<List<Models.User.User>> GetListAsync(bool load)
        {
            List<Models.User.User> user = await DataContext.Users.ToListAsync();
            if (load)
            {
                user.ForEach(async user => await Load(user));
            }
            return user;
        }

        public async Task<Models.User.User?> GetId(Guid id, bool load)
        {
            Models.User.User? user = await DataContext.Users.FirstOrDefaultAsync(user => user.user_id == id);
            if ((user != null) && load)
            { 
                await Load(user);
            }
            return user;
        }

        public async Task<Models.User.User> Load(Models.User.User user) 
        {
            user.orders = await DataContext.Orders.Where(order => order.user_id == user.user_id).ToListAsync();
            foreach (var order in user.orders)
            {
                order.user = user;
                await OrderManager.Load(order);
            }
            return user;
        }

        public async Task<Models.User.User> Save(Models.User.User user)
        {
            if(user.user_id == Guid.Empty)
            {
                user.user_id = Guid.NewGuid();
                await DataContext.Users.AddAsync(user);
            }
            else
            {
                DataContext.Users.Update(user);
            }
            await DataContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> Delete(Models.User.User user)
        {
            DataContext.Users.Remove(user);
            await DataContext.SaveChangesAsync();
            return true;
        }
    }
}
