using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Order;

namespace luizalabs.desafio_tecnico.Adapters
{
    public class OrderAdapter : IOrderAdapter
    {
        private readonly IUserAdapter UserAdapter;
        private readonly IProductAdapter ProductAdapter;

        public OrderAdapter(IUserAdapter userAdapter, IProductAdapter productAdapter) 
        { 
            UserAdapter = userAdapter;
            ProductAdapter = productAdapter;
        }

        public List<OrderView> ToListView(List<Order> itens)
        {
            return itens.Select(item => ToView(item)).ToList();
        }

        public Order ToModel(OrderInsert item)
        {
            Order order = new Order();
            order.order_id = Guid.Empty;
            order.date = item.date;

            item.products_ids.ForEach(productId => order.products.Add(new OrderProduct { product_id = productId, order = order }));

            return order;
        }

        public Order ToModel(OrderUpdate item, Order order)
        {
            order.date = item.date;
            order.products = new List<OrderProduct>();
            item.products_ids.ForEach(productId => order.products.Add(new OrderProduct { product_id = productId, order = order }));

            return order;
        }

        public OrderView ToView(Order item)
        {
            OrderView order = new OrderView();

            order.order_id = item.order_id;
            order.date = item.date;
            order.user = UserAdapter.ToView(item.user);
            order.products = ProductAdapter.ToListView(item.products.Select(orderProduct => orderProduct.product).ToList());
            order.total = item.products.Sum(orderProduct => orderProduct.product.value);

            return order;
        }
    }
}
