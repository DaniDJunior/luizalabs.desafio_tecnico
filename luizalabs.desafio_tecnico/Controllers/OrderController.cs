using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Filters;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Managers;
using luizalabs.desafio_tecnico.Models.Order;
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
        private readonly IOrderData OrderData;
        private readonly IOrderAdapter OrderAdapter;

        public OrderController(ILogger<OrderController> logger, IOrderAdapter orderAdapter, IOrderData orderData)
        {
            Logger = logger;
            OrderAdapter = orderAdapter;
            OrderData = orderData;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            return StatusCode((int)HttpStatusCode.OK, OrderAdapter.ToListView(await OrderData.GetListAsync()));
        }

        [HttpGet]
        [Route("{order_id}")]
        public async Task<IActionResult> GetId(Guid order_id)
        {
            var order = await OrderData.GetByIdAsync(order_id);
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
            order = await OrderData.SaveAsync(order);
            return StatusCode((int)HttpStatusCode.Created, OrderAdapter.ToView(order));
        }

        [HttpPost]
        [Route("{order_id}/Product")]
        public async Task<IActionResult> PostProduct([FromBody] OrderProductInsert model, Guid order_id)
        {
            Models.Order.Order? order = await OrderData.GetByIdAsync(order_id);
            if (order == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            var orderProduct = OrderAdapter.ToProductModel(model, order);
            order = await OrderData.AddProductAsync(order, orderProduct);
            return StatusCode((int)HttpStatusCode.Created, OrderAdapter.ToView(order));
        }

        [HttpPut]
        [Route("{order_id}")]
        public async Task<IActionResult> Put([FromBody] OrderUpdate model, Guid order_id)
        {
            Models.Order.Order? order = await OrderData.GetByIdAsync(order_id);
            if (order == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            order = OrderAdapter.ToModel(model, order);
            order = await OrderData.SaveAsync(order);
            return StatusCode((int)HttpStatusCode.OK, OrderAdapter.ToView(order));
        }

        [HttpDelete]
        [Route("{order_id}")]
        public async Task<IActionResult> Delete(Guid order_id)
        {
            Models.Order.Order? order = await OrderData.GetByIdAsync(order_id);
            if (order == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            if (await OrderData.DeleteAsync(order))
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
