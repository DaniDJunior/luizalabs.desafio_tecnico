using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace luizalabs.desafio_tecnico.Models.Order
{
    public class OrderProduct
    {
        [Key]
        public Guid product_id { get; set; }

        public int? legacy_product_id { get; set; }

        public Guid order_id { get; set; }
        [ForeignKey(nameof(order_id))]
        [InverseProperty("products")]
        public virtual Order order { get; set; }

        public float value { get; set; }

        public OrderProduct()
        {
            product_id = Guid.Empty;
            order = new Order();
        }
    }
}
