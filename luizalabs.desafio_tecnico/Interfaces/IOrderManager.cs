namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface IOrderManager
    {
        public Task<List<Models.Order.Order>> GetListAsync();

        public Task<Models.Order.Order?> GetId(Guid id);

        public Task<Models.Order.Order> Load(Models.Order.Order order);

        public Task<Models.Order.Order> Save(Models.Order.Order order);

        public Task<bool> Delete(Models.Order.Order order);
    }
}
