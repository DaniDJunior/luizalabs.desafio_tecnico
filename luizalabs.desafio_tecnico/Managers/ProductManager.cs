using luizalabs.desafio_tecnico.Data;
using luizalabs.desafio_tecnico.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace luizalabs.desafio_tecnico.Managers
{
    public class ProductManager : IProductManager
    {
        private readonly ILogger<ProductManager> Logger;
        private readonly DataContext DataContext;

        public ProductManager(ILogger<ProductManager> logger, DataContext dataContext)
        {
            Logger = logger;
            DataContext = dataContext;
        }

        public async Task<List<Models.Product.Product>> GetListAsync()
        {
            return await DataContext.Products.ToListAsync();
        }

        public async Task<Models.Product.Product?> GetId(Guid id)
        {
            return await DataContext.Products.FirstOrDefaultAsync(product => product.product_id == id);
        }

        public async Task<Models.Product.Product> Save(Models.Product.Product product)
        {
            if (product.product_id == Guid.Empty)
            {
                product.product_id = Guid.NewGuid();
                await DataContext.Products.AddAsync(product);
            }
            else
            {
                DataContext.Products.Update(product);
            }
            await DataContext.SaveChangesAsync();
            return product;
        }

        public async Task<bool> Delete(Models.Product.Product product)
        {
            DataContext.Products.Remove(product);
            await DataContext.SaveChangesAsync();
            return true;
        }
    }
}
