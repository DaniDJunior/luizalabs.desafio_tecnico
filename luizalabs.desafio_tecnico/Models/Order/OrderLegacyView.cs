namespace luizalabs.desafio_tecnico.Models.Order
{
    public class OrderLegacyView
    {
        public int? order_id { get; set; }
        public DateTime date { get; set; }
        public float total { get; set; }
        public List<OrderProductLegacyView> products { get; set; }

        public OrderLegacyView()
        {
            products = new List<OrderProductLegacyView>();
        }
    }
}
