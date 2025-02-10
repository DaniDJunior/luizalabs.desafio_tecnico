using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Models.Order;
using luizalabs.desafio_tecnico.test.LocalMock.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luizalabs.desafio_tecnico.test.Adapters
{
    [TestClass]
    public class OrderAdapterTest
    {
        private OrderAdapter OrderAdapter;

        [TestInitialize]
        public void TestInitialize()
        {   
            OrderAdapter = new OrderAdapter();
        }

        [TestMethod]
        public void ToView()
        {
            Order order = OrderSample.Sample();
            OrderView productVies = OrderAdapter.ToView(order);

            Assert.AreEqual(order.order_id, productVies.order_id);
            Assert.AreEqual(order.date, productVies.date);
        }

        [TestMethod]
        public void ToViewProduct()
        {
            //Order order = OrderSample.Sample();

            //int sampleSize = 5;

            //float total = 0;

            //for (int i = 0; i < sampleSize; i++)
            //{
            //    OrderProduct product = ProductSample.Sample();

            //    total += product.value;

            //    OrderProduct orderProduct = new OrderProduct();
            //    orderProduct.product = product;
            //    orderProduct.product_id = product.product_id;
            //    orderProduct.order = order;
            //    orderProduct.order_id = order.order_id;
            //    order.products.Add(orderProduct);
            //}

            //OrderView productVies = OrderAdapter.ToView(order);

            //Assert.AreEqual(order.order_id, productVies.order_id);
            //Assert.AreEqual(order.date, productVies.date);
            //Assert.AreEqual(sampleSize, productVies.products.Count);
            //Assert.AreEqual(total, productVies.total);
        }

        [TestMethod]
        public void ToListView()
        {
            int sampleSize = 5;

            List<Order> orders = new List<Order>();

            for (int i = 0; i < sampleSize; i++)
            {
                orders.Add(OrderSample.Sample());
            }

            List<OrderView> ordersVies = OrderAdapter.ToListView(orders);

            Assert.AreEqual(orders.Count, ordersVies.Count);

            for (int i = 0; i < sampleSize; i++)
            {
                Assert.AreEqual(orders[i].order_id, ordersVies[i].order_id);
                Assert.AreEqual(orders[i].date, ordersVies[i].date);
            }
        }

        [TestMethod]
        public void ToModelInsert()
        {
            OrderInsert orderInsert = OrderSample.SampleInsert();
            Order order = OrderAdapter.ToModel(orderInsert);

            Assert.AreEqual(order.order_id, Guid.Empty);
            Assert.AreEqual(order.date, orderInsert.date);
        }

        [TestMethod]
        public void ToModelUpdate()
        {
            Order orderInit = OrderSample.Sample();
            OrderUpdate orderUpdate = OrderSample.SampleUpdate();
            Order order = OrderAdapter.ToModel(orderUpdate, orderInit);

            Assert.AreEqual(order.order_id, orderInit.order_id);
            Assert.AreEqual(order.date, orderUpdate.date);
        }
    }
}
