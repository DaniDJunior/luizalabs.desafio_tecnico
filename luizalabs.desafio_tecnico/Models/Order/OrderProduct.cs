using Microsoft.AspNetCore.Routing.Constraints;

namespace luizalabs.desafio_tecnico.Models.Order
{
    public class OrderProduct
    {
        public Guid order_id { get; set; }
        public virtual Order? order { get; set; }
        public Guid product_id { get; set; }
        public virtual Product.Product? product { get; set; }
    }
}
