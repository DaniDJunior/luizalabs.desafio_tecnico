using System.ComponentModel.DataAnnotations;
using System.Data;

namespace luizalabs.desafio_tecnico.Models.User
{
    public class User
    {
        [Key]
        public Guid user_id { get; set; }

        public int? legacy_user_id { get; set; }

        public string name { get; set; }

        public virtual ICollection<Order.Order> orders { get; set; }

        public User()
        {
            name = string.Empty;
            orders = new List<Order.Order>();
        }
    }
}
