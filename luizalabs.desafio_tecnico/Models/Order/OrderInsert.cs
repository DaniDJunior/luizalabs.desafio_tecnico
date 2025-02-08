namespace luizalabs.desafio_tecnico.Models.Order
{
    public class OrderInsert
    {
        public DateTime date { get; set; }
        public List<Guid> products_ids { get; set; }

        public OrderInsert()
        {
            products_ids = new List<Guid>();
        }
    }
}
