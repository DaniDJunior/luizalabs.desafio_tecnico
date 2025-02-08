using luizalabs.desafio_tecnico.Models.Product;
using luizalabs.desafio_tecnico.Models.User;

namespace luizalabs.desafio_tecnico.Models.Order
{
    public class OrderView
    {
        public Guid order_id { get; set; }
        public DateTime date { get; set; }
        public float total { get; set; }
        public UserView user { get; set; }
        public List<ProductView> products { get; set; }

        public OrderView() 
        { 
            products = new List<ProductView>();
            user = new UserView();
        }
    }
}
