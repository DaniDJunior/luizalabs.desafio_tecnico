using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Order;
using static Confluent.Kafka.ConfigPropertyNames;

namespace luizalabs.desafio_tecnico.Adapters
{
    public class OrderAdapter : IOrderAdapter
    {
        public Order ToModel(OrderInsert item)
        {
            Order order = new Order();
            order.order_id = Guid.Empty;
            order.date = item.date;

            item.products.ForEach(product => order.products.Add(new OrderProduct { product_id = Guid.Empty, value = product.value, order = order, order_id = order.order_id }));

            return order;
        }

        public OrderProduct ToProductModel(OrderProductInsert item, Order order)
        {
            OrderProduct orderProduct = new OrderProduct();
            orderProduct.order_id = order.order_id;
            orderProduct.order = order;
            orderProduct.product_id = Guid.Empty;
            orderProduct.value = item.value;

            return orderProduct;
        }

        public List<OrderView> ToListView(List<Order> itens)
        {
            return itens.Select(item => ToView(item)).ToList();
        }

        public Order ToModel(OrderUpdate item, Order order)
        {
            order.date = item.date;

            return order;
        }

        public List<OrderLegacyView> ToListLegacyView(List<Order> itens)
        {
            return itens.Select(item => ToLegacyView(item)).ToList();
        }

        public OrderView ToView(Order item)
        {
            OrderView order = new OrderView();

            order.order_id = item.order_id;
            order.date = item.date;
            order.products = ToProductListView(item.products.ToList());
            order.total = item.products.Sum(p => p.value);

            return order;
        }

        public OrderLegacyView ToLegacyView(Order item)
        {
            OrderLegacyView order = new OrderLegacyView();

            order.order_id = item.legacy_order_id;
            order.date = item.date;
            order.products = ToProductLegacyListView(item.products.ToList());
            order.total = item.products.Sum(p => p.value);

            return order;
        }

        private List<OrderProductView> ToProductListView(List<OrderProduct> itens)
        {
            return itens.Select(item => ToProductView(item)).ToList();
        }

        private OrderProductView ToProductView(OrderProduct item)
        {
            OrderProductView order = new OrderProductView();

            order.product_id = item.product_id;
            order.value = item.value;

            return order;
        }

        private List<OrderProductLegacyView> ToProductLegacyListView(List<OrderProduct> itens)
        {
            return itens.Select(item => ToProductLegacyView(item)).ToList();
        }

        private OrderProductLegacyView ToProductLegacyView(OrderProduct item)
        {
            OrderProductLegacyView order = new OrderProductLegacyView();

            order.product_id = item.legacy_product_id;
            order.value = item.value;

            return order;
        }
    }
}
