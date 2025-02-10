using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Order;
using luizalabs.desafio_tecnico.Models.User;
using Microsoft.EntityFrameworkCore;

namespace luizalabs.desafio_tecnico.Data
{
    public class OrderData : IOrderData
    {
        private readonly ILogger<OrderData> Logger;
        private readonly IServiceProvider ServiceProvider;

        public OrderData(ILogger<OrderData> logger, IServiceProvider serviceProvider)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
        }

        public async Task<List<Order>> GetListAsync()
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                
                List<Order> orders = await dataContext.Orders.Include(o => o.user).Include(o => o.products).ToListAsync();
                return orders;
            }
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                Order? order = await dataContext.Orders.Include(o => o.user).Include(o => o.products).FirstOrDefaultAsync(order => order.order_id == id);
                return order;
            }
        }

        public async Task<Order> SaveAsync(Order order)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                List<OrderProduct> orderProductsDataBase = new List<OrderProduct>();
                if (order.order_id == Guid.Empty)
                {
                    order.order_id = Guid.NewGuid();
                    dataContext.Entry(order).State = EntityState.Added;
                }
                else
                {
                    dataContext.Entry(order).State = EntityState.Modified;
                }

                await dataContext.SaveChangesAsync();
                return order;
            }
        }

        public async Task<Order> AddProductAsync(Order order, OrderProduct product)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                product.product_id = Guid.NewGuid();
                product.order = order;
                product.order_id = order.order_id;
                
                order.products.Add(product);
                
                dataContext.Entry(product).State = EntityState.Added;

                await dataContext.SaveChangesAsync();
                return order;
            }
        }

        public async Task<bool> DeleteAsync(Order order)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                dataContext.Orders.Remove(order);
                await dataContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<Order?> GetByIdLegacyAsync(int id)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                Order? order = await dataContext.Orders.Include(o => o.user).Include(o => o.products).FirstOrDefaultAsync(order => order.legacy_order_id == id);
                return order;
            }
        }
    }
}
