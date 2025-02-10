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

        public static Models.Order.OrderInsert SampleInsert()
        {
            Models.Order.OrderInsert order = new Models.Order.OrderInsert();

            order.date = DateTime.Now;

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
