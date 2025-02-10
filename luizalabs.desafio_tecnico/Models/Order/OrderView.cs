using luizalabs.desafio_tecnico.Models.User;

namespace luizalabs.desafio_tecnico.Models.Order
{
    public class OrderView
    {
        public Guid order_id { get; set; }
        public DateTime date { get; set; }
        public float total { get; set; }
        public List<OrderProductView> products { get; set; }

        public OrderView() 
        { 
            products = new List<OrderProductView>();
        }
    }
}
