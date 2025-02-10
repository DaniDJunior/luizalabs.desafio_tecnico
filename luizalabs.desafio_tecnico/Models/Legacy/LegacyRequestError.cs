using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace luizalabs.desafio_tecnico.Models.Legacy
{
    public class LegacyRequestError
    {
        [Key]
        public Guid request_error_id { get; set; }

        [ForeignKey(nameof(request_id))]
        [InverseProperty("errors")]
        public virtual LegacyRequest request { get; set; }
        public Guid request_id { get; set; }

        public int level { get; set; }
        public int line_number { get; set; }
        public string message { get; set; }

        public LegacyRequestError() 
        {
            message = string.Empty;
            request = new LegacyRequest();
        }
    }
}
