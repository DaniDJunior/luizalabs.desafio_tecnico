using System.ComponentModel.DataAnnotations;
using System.Data;

namespace luizalabs.desafio_tecnico.Models.Order
{
    public class Order
    {
        [Key]
        public Guid order_id { get; set; }

        public int? legacy_order_id { get; set; }

        public DateTime date { get; set; }
        public virtual ICollection<OrderProduct> products { get; set; }

        public virtual User.User user { get; set; }
        public Guid user_id { get; set; }

        public Order()
        {
            user = new User.User();
            products = new List<OrderProduct>();
        }
    }
}
