using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Filters;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Managers;
using luizalabs.desafio_tecnico.Models.Order;
using luizalabs.desafio_tecnico.Models.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace luizalabs.desafio_tecnico.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [AuthFilter]
    [Route("api/v{version:apiVersion}/Order")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> Logger;
        private readonly IOrderManager OrderManager;
        private readonly IOrderAdapter OrderAdapter;

        public OrderController(ILogger<OrderController> logger, IOrderAdapter orderAdapter, IOrderManager orderManager)
        {
            Logger = logger;
            OrderAdapter = orderAdapter;
            OrderManager = orderManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            return StatusCode((int)HttpStatusCode.OK, OrderAdapter.ToListView(await OrderManager.GetListAsync()));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetId(Guid id)
        {
            var order = await OrderManager.GetId(id);
            if (order == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.OK, OrderAdapter.ToView(order));
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Post([FromBody] OrderInsert model)
        {
            var order = OrderAdapter.ToModel(model);
            order = await OrderManager.Save(order);
            return StatusCode((int)HttpStatusCode.Created, OrderAdapter.ToView(order));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put([FromBody] OrderUpdate model, Guid id)
        {
            Models.Order.Order? order = await OrderManager.GetId(id);
            if (order == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            order = OrderAdapter.ToModel(model, order);
            order = await OrderManager.Save(order);
            return StatusCode((int)HttpStatusCode.OK, OrderAdapter.ToView(order));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Models.Order.Order? order = await OrderManager.GetId(id);
            if (order == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            if (await OrderManager.Delete(order))
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
