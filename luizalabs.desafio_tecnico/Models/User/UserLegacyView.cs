using luizalabs.desafio_tecnico.Models.Order;

namespace luizalabs.desafio_tecnico.Models.User
{
    public class UserLegacyView
    {
        public int? user_id { get; set; }
        public string name { get; set; }
        public List<OrderLegacyView> orders { get; set; }

        public UserLegacyView()
        {
            name = string.Empty;
            orders = new List<OrderLegacyView>();
        }
    }
}
