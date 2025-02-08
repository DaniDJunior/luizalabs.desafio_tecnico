namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface IOrderAdapter
    {
        public List<Models.Order.OrderView> ToListView(List<Models.Order.Order> itens);

        public Models.Order.OrderView ToView(Models.Order.Order item);

        public Models.Order.Order ToModel(Models.Order.OrderInsert item);

        public Models.Order.Order ToModel(Models.Order.OrderUpdate item, Models.Order.Order order);
    }
}
