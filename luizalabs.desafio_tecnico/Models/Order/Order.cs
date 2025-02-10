using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace luizalabs.desafio_tecnico.Models.Order
{
    public class Order
    {
        [Key]
        public Guid order_id { get; set; }

        public int? legacy_order_id { get; set; }

        public DateTime date { get; set; }

        [InverseProperty(nameof(OrderProduct.order))]
        public virtual ICollection<OrderProduct> products { get; set; }

        [ForeignKey(nameof(user_id))]
        [InverseProperty("orders")]
        public virtual User.User user { get; set; }
        public Guid user_id { get; set; }

        public Order()
        {
            order_id = Guid.Empty;
            user = new User.User();
            products = new List<OrderProduct>();
        }
    }
}
