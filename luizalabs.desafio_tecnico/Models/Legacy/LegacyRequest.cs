using luizalabs.desafio_tecnico.Models.Order;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace luizalabs.desafio_tecnico.Models.Legacy
{
    public class LegacyRequest
    {
        [Key]
        public Guid request_id { get; set; }
        public string file_name { get; set; }
        public int total_lines { get; set; }
        public string status { get; set; }

        [InverseProperty(nameof(LegacyRequestLine.request))]
        public virtual ICollection<LegacyRequestLine> lines { get; set; }

        [InverseProperty(nameof(LegacyRequestError.request))]
        public virtual ICollection<LegacyRequestError> errors { get; set; }

        public LegacyRequest()
        {
            request_id = Guid.Empty;
            file_name = string.Empty;
            status = string.Empty;
            lines = new List<LegacyRequestLine>();
            errors = new List<LegacyRequestError>();
        }

    }
}
