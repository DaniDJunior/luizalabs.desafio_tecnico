using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luizalabs.desafio_tecnico.test.LocalMock.Sample
{
    internal class OrderSample
    {
        public static Models.Order.Order Sample()
        {
            Models.Order.Order order = new Models.Order.Order();

            order.user_id = Guid.NewGuid();
            order.date = DateTime.Now;
            order.legacy_order_id = ComunSample.RandomInt();

            return order;
        }

        public static Models.Order.Order SampleAddProduct(Models.Order.Order order)
        {
            Models.Order.OrderProduct orderProduct = new Models.Order.OrderProduct();

            orderProduct.legacy_product_id = ComunSample.RandomInt();
            orderProduct.value = ComunSample.RandomFloat();
            orderProduct.order = order;
            orderProduct.order_id = order.order_id;
            orderProduct.product_id = Guid.NewGuid();

            order.products.Add(orderProduct);

            return order;
        }

        public static Models.Order.OrderInsert SampleInsert()
        {
            Models.Order.OrderInsert order = new Models.Order.OrderInsert();

            order.date = DateTime.Now;

            return order;
        }

        public static Models.Order.OrderInsert SampleAddProductInsert(Models.Order.OrderInsert order)
        {
            Models.Order.OrderProductInsert orderProduct = new Models.Order.OrderProductInsert();

            orderProduct.value = ComunSample.RandomFloat();

            order.products.Add(orderProduct);

            return order;
        }

        public static Models.Order.OrderUpdate SampleUpdate()
        {
            Models.Order.OrderUpdate order = new Models.Order.OrderUpdate();

            order.date = DateTime.Now;

            return order;
        }
    }
}
