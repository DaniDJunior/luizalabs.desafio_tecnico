using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Product;

namespace luizalabs.desafio_tecnico.Adapters
{
    public class ProductAdapter : IProductAdapter
    {
        public List<ProductView> ToListView(List<Product> itens)
        {
            return itens.Select(item => ToView(item)).ToList();
        }

        public Product ToModel(ProductInsert item)
        {
            Product product = new Product();
            product.product_id = Guid.Empty;
            product.value = item.value;

            return product;
        }

        public Product ToModel(ProductUpdate item, Product product)
        {
            product.value = item.value;

            return product;
        }

        public ProductView ToView(Product item)
        {
            ProductView product = new ProductView();

            product.product_id = item.product_id;
            product.value = item.value;

            return product;
        }
    }
}
