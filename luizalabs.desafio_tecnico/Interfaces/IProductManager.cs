namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface IProductManager
    {
        public Task<List<Models.Product.Product>> GetListAsync();

        public Task<Models.Product.Product?> GetId(Guid id);

        public Task<Models.Product.Product> Save(Models.Product.Product product);

        public Task<bool> Delete(Models.Product.Product product);
    }
}
