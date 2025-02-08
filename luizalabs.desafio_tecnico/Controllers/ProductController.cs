using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Filters;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Managers;
using luizalabs.desafio_tecnico.Models.Product;
using luizalabs.desafio_tecnico.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace luizalabs.desafio_tecnico.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [AuthFilter]
    [Route("api/v{version:apiVersion}/Product")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> Logger;
        private readonly IProductManager ProductManager;
        private readonly IProductAdapter ProductAdapter;

        public ProductController(ILogger<ProductController> logger, IProductAdapter productAdapter, IProductManager productManager)
        {
            Logger = logger;
            ProductAdapter = productAdapter;
            ProductManager = productManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            return StatusCode((int)HttpStatusCode.OK, ProductAdapter.ToListView(await ProductManager.GetListAsync()));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetId(Guid id)
        {
            var product = await ProductManager.GetId(id);
            if (product == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.OK, ProductAdapter.ToView(product));
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Post([FromBody] ProductInsert model)
        {
            var product = ProductAdapter.ToModel(model);
            product = await ProductManager.Save(product);
            return StatusCode((int)HttpStatusCode.Created, ProductAdapter.ToView(product));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put([FromBody] ProductUpdate model, Guid id)
        {
            Models.Product.Product? product = await ProductManager.GetId(id);
            if (product == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            product = ProductAdapter.ToModel(model, product);
            product = await ProductManager.Save(product);
            return StatusCode((int)HttpStatusCode.OK, ProductAdapter.ToView(product));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Models.Product.Product? product = await ProductManager.GetId(id);
            if (product == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            if (await ProductManager.Delete(product))
            {
                return StatusCode((int)HttpStatusCode.OK);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
