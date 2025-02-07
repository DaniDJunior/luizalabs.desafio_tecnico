using System.ComponentModel.DataAnnotations;

namespace luizalabs.desafio_tecnico.Models.Legacy
{
    public class LegacyData
    {
        [Key]
        public Guid request_line_id { get; set; }
        public int line { get; set; }
        public virtual LegacyFile file { get; set; }
        public Guid request_id { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public float product_value { get; set; }
        public DateTime order_date { get; set; }
    }
}
