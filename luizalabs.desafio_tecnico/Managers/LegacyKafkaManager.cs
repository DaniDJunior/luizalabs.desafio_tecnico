using luizalabs.desafio_tecnico.Data;
using luizalabs.desafio_tecnico.Enuns;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Globalization;

namespace luizalabs.desafio_tecnico.Managers
{
    public class LegacyKafkaManager : ILegacyKafkaManager
    {
        private ILogger<LegacyKafkaManager> Logger;
        private IServiceProvider ServiceProvider;
        private readonly IKafkaManager KafkaManager;

        public LegacyKafkaManager(ILogger<LegacyKafkaManager> logger, IServiceProvider serviceProvider, IKafkaManager kafkaManager)
        {
            ServiceProvider = serviceProvider;
            Logger = logger;
            KafkaManager = kafkaManager;
        }

        public async Task<object> ProcessFileAsync(string fileName, Guid requestId)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                Models.Legacy.LegacyRequest request = await dataContext.Requests.FirstAsync(x => x.request_id == requestId);

                request.status = LegacyFileStatus.PROCESSED;

                dataContext.Requests.Update(request);

                await dataContext.SaveChangesAsync();

                string[] lines = await System.IO.File.ReadAllLinesAsync(fileName);

                for (int i = 0; i < lines.Length; i++)
                {
                    await KafkaManager.SendProcessLineAsync(lines[i], i, requestId);
                }
            }
            return Task.CompletedTask;
        }

        public async Task<object> CheckProcessFinal(Guid requestId)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                Models.Legacy.LegacyRequest request = await dataContext.Requests.FirstAsync(x => x.request_id == requestId);

                int lines_process = await dataContext.RequestsLines.CountAsync(x => x.request_id == requestId);

                if (request.total_lines == lines_process)
                {
                    await KafkaManager.SendProcessFinalAsync(requestId);
                }

            }
            return Task.CompletedTask;
        }

        public async Task<object> ProcessLineAsync(string line, int linePosition, Guid requestId)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                Models.Legacy.LegacyRequest request = await dataContext.Requests.FirstAsync(x => x.request_id == requestId);

                string line_user_id = line.Substring(0, 10);
                string line_user_name = line.Substring(10, 45);
                string line_order_id = line.Substring(55, 10);
                string line_product_id = line.Substring(65, 10);
                string line_product_value = line.Substring(75, 12);
                string line_order_date = line.Substring(87, 8);

                int user_id = 0;
                string user_name = string.Empty;
                int order_id = 0;
                int product_id = 0;
                float product_value = 0;
                DateTime order_date = DateTime.Now;

                if (!int.TryParse(line_user_id, out user_id))
                {
                    return Task.CompletedTask;
                }

                user_name = line_user_name.Trim();

                if (!int.TryParse(line_order_id, out order_id))
                {
                    return Task.CompletedTask;
                }

                if (!int.TryParse(line_product_id, out product_id))
                {
                    return Task.CompletedTask;
                }

                if (!float.TryParse(line_product_value.Trim(), out product_value))
                {
                    return Task.CompletedTask;
                }

                if (!DateTime.TryParseExact(line_order_date, "yyyyMMdd", null, DateTimeStyles.None, out order_date))
                {
                    return Task.CompletedTask;
                }

                Models.Legacy.LegacyRequestLine? request_data = await dataContext.RequestsLines.FirstOrDefaultAsync(line => line.request_id == request.request_id && line.line_number == linePosition);

                bool flagUpdate = request_data != null;

                if (request_data == null) 
                {
                    request_data = new Models.Legacy.LegacyRequestLine();
                    request_data.request_line_id = Guid.NewGuid();
                }
                
                request_data.line_number = linePosition;
                request_data.request_id = request.request_id;

                request_data.user_id = user_id;
                request_data.user_name = user_name;
                request_data.order_id = order_id;
                request_data.product_id = product_id;
                request_data.product_value = product_value;
                request_data.order_date = order_date;

                if (flagUpdate)
                {
                    dataContext.RequestsLines.Update(request_data);
                }
                else
                {
                    dataContext.RequestsLines.Add(request_data);
                    request.Lines.Add(request_data);
                }
                
                dataContext.Requests.Update(request);

                await dataContext.SaveChangesAsync();

                await KafkaManager.SendProcessDataAsync(request_data.request_line_id);
            }

            await CheckProcessFinal(requestId);

            return Task.CompletedTask;
        }

        public async Task<object> ProcessFinalAsync(Guid requestId)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                Models.Legacy.LegacyRequest request = await dataContext.Requests.FirstAsync(x => x.request_id == requestId);

                request.status = LegacyFileStatus.COMPLETE;

                await dataContext.SaveChangesAsync();

            }
            return Task.CompletedTask;
        }

        public async Task<object> ProcessDataAsync(Guid requestLineId)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                Models.Legacy.LegacyRequestLine requestLine = await dataContext.RequestsLines.FirstAsync(x => x.request_line_id == requestLineId);

                Models.User.User? user = await dataContext.Users.FirstOrDefaultAsync(u => u.legacy_user_id == requestLine.user_id);
                Models.Order.Order? order = await dataContext.Orders.FirstOrDefaultAsync(o => o.legacy_order_id == requestLine.order_id);
                Models.Product.Product? product = await dataContext.Products.FirstOrDefaultAsync(p => p.legacy_product_id == requestLine.product_id);

                if (user == null)
                {
                    user = new Models.User.User();
                    user.user_id = Guid.NewGuid();
                    user.legacy_user_id = requestLine.user_id;
                    user.name = requestLine.user_name;

                    await dataContext.Users.AddAsync(user);
                }
                else
                {
                    if (user.name != requestLine.user_name) 
                    { 
                        /// TODO: Divergencia Nome
                    }
                }

                if (product == null) 
                {
                    product = new Models.Product.Product();
                    product.product_id = Guid.NewGuid();
                    product.legacy_product_id = requestLine.product_id;
                    product.value = requestLine.product_value;

                    await dataContext.Products.AddAsync(product);
                }
                else
                {
                    if (product.value != requestLine.product_value)
                    {
                        /// TODO: Divergencia Value
                    }
                }

                if (order == null)
                {
                    order = new Models.Order.Order();
                    order.order_id = Guid.NewGuid();
                    order.legacy_order_id = requestLine.order_id;
                    order.date = requestLine.order_date;
                    order.user = user;
                    order.user_id = user.user_id;

                    user.orders.Add(order);

                    await dataContext.Orders.AddAsync(order);
                }
                else
                {
                    if (order.date != requestLine.order_date)
                    {
                        /// TODO: Divergencia Date
                    }
                }

                Models.Order.OrderProduct? orderProduct = await dataContext.OrdersProducts.FirstOrDefaultAsync(op => op.order_id == order.order_id && op.product_id == product.product_id);

                if (orderProduct == null)
                {
                    orderProduct = new Models.Order.OrderProduct();
                    orderProduct.product = product;
                    orderProduct.product_id = product.product_id;
                    orderProduct.order = order;
                    orderProduct.order_id = order.order_id;

                    await dataContext.OrdersProducts.AddAsync(orderProduct);

                    order.products.Add(orderProduct);
                }

                await dataContext.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }
    }
}
