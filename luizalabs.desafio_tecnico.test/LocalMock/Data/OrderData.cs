using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luizalabs.desafio_tecnico.test.LocalMock.Data
{
    internal class OrderData : IOrderData
    {
        private List<Order> Orders { get; set; }

        public OrderData()
        {
            Orders = new List<Order>();
        }

        public async Task<bool> DeleteAsync(Order order)
        {
            Orders.Remove(order);
            return true;
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return Orders.FirstOrDefault(u => u.order_id == id);
        }

        public async Task<Order?> GetByIdLegacyAsync(int id)
        {
            return Orders.FirstOrDefault(u => u.legacy_order_id == id);
        }

        public async Task<List<Order>> GetListAsync()
        {
            return Orders;
        }

        public async Task<Order> SaveAsync(Order order)
        {
            if (!Orders.Contains(order))
            {
                order.order_id = Guid.NewGuid();
                Orders.Add(order);
            }
            return order;
        }

        public async Task<Order> LoadAsync(Order order)
        {
            return order;
        }

        public async Task<Order> AddProductAsync(Order order, OrderProduct product)
        {
            order.products.Add(product);
            return order;
        }
    }
}
