using luizalabs.desafio_tecnico.Models.Order;

namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface IOrderAdapter
    {
        public List<OrderLegacyView> ToListLegacyView(List<Order> itens);

        public List<OrderView> ToListView(List<Order> itens);

        public OrderView ToView(Order item);

        public OrderLegacyView ToLegacyView(Order item);

        public OrderProduct ToProductModel(OrderProductInsert item, Order order);

        public Order ToModel(OrderInsert item);

        public Order ToModel(OrderUpdate item, Order order);
    }
}
