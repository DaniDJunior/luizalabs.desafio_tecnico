namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface IProductAdapter
    {
        public List<Models.Product.ProductView> ToListView(List<Models.Product.Product> itens);

        public Models.Product.ProductView ToView(Models.Product.Product item);

        public Models.Product.Product ToModel(Models.Product.ProductInsert item);

        public Models.Product.Product ToModel(Models.Product.ProductUpdate item, Models.Product.Product product);
    }
}
