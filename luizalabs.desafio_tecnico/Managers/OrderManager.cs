using luizalabs.desafio_tecnico.Data;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Order;
using luizalabs.desafio_tecnico.Models.User;
using Microsoft.EntityFrameworkCore;

namespace luizalabs.desafio_tecnico.Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly ILogger<OrderManager> Logger;
        private readonly DataContext DataContext;
        private readonly IProductManager ProductManager;

        public OrderManager(ILogger<OrderManager> logger, DataContext dataContext, IProductManager productManager)
        {
            Logger = logger;
            DataContext = dataContext;
            ProductManager = productManager;
        }

        public async Task<List<Models.Order.Order>> GetListAsync()
        {
            List<Models.Order.Order> orders = await DataContext.Orders.ToListAsync();
            orders.ForEach(async orders => await Load(orders));
            return orders;
        }

        public async Task<Models.Order.Order?> GetId(Guid id)
        {
            Models.Order.Order? order = await DataContext.Orders.FirstOrDefaultAsync(order => order.order_id == id);
            if (order != null)
            {
                await Load(order);
            }
            return order;
        }

        public async Task<Models.Order.Order> Load(Models.Order.Order order)
        {
            order.products = await DataContext.OrdersProducts.Where(o => o.order_id == order.order_id).ToListAsync();
            foreach (var orderProduct in order.products)
            {
                orderProduct.order = order;
                orderProduct.product = await ProductManager.GetId(orderProduct.product_id);
            }
            return order;
        }

        public async Task<Models.Order.Order> Save(Models.Order.Order order)
        {
            List<OrderProduct> orderProductsDataBase = new List<OrderProduct>();
            if (order.order_id == Guid.Empty)
            {
                order.order_id = Guid.NewGuid();
                await DataContext.Orders.AddAsync(order);
            }
            else
            {
                orderProductsDataBase = await DataContext.OrdersProducts.Where(op => op.order_id == order.order_id).ToListAsync();
                DataContext.Orders.Update(order);
            }

            foreach (var orderProduct in order.products)
            {
                if(orderProductsDataBase.FirstOrDefault(op => op.product_id == orderProduct.product_id) == null)
                {
                    orderProduct.product = await ProductManager.GetId(orderProduct.product_id);
                    orderProduct.order = order;
                    orderProduct.order_id = order.order_id;
                    order.products.Add(orderProduct);
                    DataContext.OrdersProducts.Add(orderProduct);
                }
            }

            foreach (var orderProduct in orderProductsDataBase)
            {
                if(order.products.FirstOrDefault(op => op.product_id == orderProduct.product_id) == null)
                {
                    order.products.Remove(orderProduct);
                    DataContext.OrdersProducts.Remove(orderProduct);
                }
            }

            await DataContext.SaveChangesAsync();
            return order;
        }

        public async Task<bool> Delete(Models.Order.Order order)
        {
            DataContext.Orders.Remove(order);
            await DataContext.SaveChangesAsync();
            return true;
        }
    }
}
