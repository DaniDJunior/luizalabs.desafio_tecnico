using luizalabs.desafio_tecnico.Data;
using luizalabs.desafio_tecnico.Models.Order;

namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface IOrderData
    {
        public Task<List<Models.Order.Order>> GetListAsync();

        public Task<Models.Order.Order?> GetByIdAsync(Guid id);

        public Task<Models.Order.Order?> GetByIdLegacyAsync(int id);

        public Task<Models.Order.Order> SaveAsync(Models.Order.Order order);

        public Task<Order> AddProductAsync(Order order, OrderProduct product);

        public Task<bool> DeleteAsync(Models.Order.Order order);
    }
}
