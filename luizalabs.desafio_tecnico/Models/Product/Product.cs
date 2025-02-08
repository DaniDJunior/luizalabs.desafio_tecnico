using System.ComponentModel.DataAnnotations;

namespace luizalabs.desafio_tecnico.Models.Product
{
    public class Product
    {
        [Key]
        public Guid product_id { get; set; }

        public int? legacy_product_id {  get; set; }

        public float value { get; set; }
    }
}
