using System.ComponentModel.DataAnnotations;

namespace luizalabs.desafio_tecnico.Models.Legacy
{
    public class LegacyRequestLine
    {
        [Key]
        public Guid request_line_id { get; set; }
        
        public virtual LegacyRequest? request { get; set; }
        public Guid request_id { get; set; }

        public int line_number { get; set; }
        
        public int user_id { get; set; }
        public string user_name { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public float product_value { get; set; }
        public DateTime order_date { get; set; }

        public LegacyRequestLine()
        {
            user_name = string.Empty;
        }
    }
}
