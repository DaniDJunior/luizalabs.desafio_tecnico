namespace luizalabs.desafio_tecnico.Models.Order
{
    public class OrderInsert
    {
        public DateTime date { get; set; }
        public List<OrderProductInsert> products { get; set; } 

        public OrderInsert()
        {
            products = new List<OrderProductInsert>();
        }
    }
}
