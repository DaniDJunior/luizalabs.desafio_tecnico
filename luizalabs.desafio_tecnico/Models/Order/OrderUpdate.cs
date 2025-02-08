namespace luizalabs.desafio_tecnico.Models.Order
{
    public class OrderUpdate
    {
        public DateTime date { get; set; }
        public List<Guid> products_ids { get; set; }

        public OrderUpdate() 
        {
            products_ids = new List<Guid>();
        }
    }
}
