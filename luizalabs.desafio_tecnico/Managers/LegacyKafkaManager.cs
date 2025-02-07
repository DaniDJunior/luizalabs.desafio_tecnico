using luizalabs.desafio_tecnico.Data;
using luizalabs.desafio_tecnico.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;

namespace luizalabs.desafio_tecnico.Managers
{
    public class LegacyKafkaManager : ILegacyKafkaManager
    {
        private ILogger<LegacyKafkaManager> Logger;
        private IServiceProvider ServiceProvider;

        public LegacyKafkaManager(ILogger<LegacyKafkaManager> logger, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Logger = logger;
        }

        public async Task<object> ProcessLineAsync(string line, int line_position, Guid requestId)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                
                Models.Legacy.LegacyFile request = await dataContext.Requests.FirstAsync(x => x.request_id == requestId);

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

                user_name = line_user_id.Trim();

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

                Models.Legacy.LegacyData request_data = new Models.Legacy.LegacyData();

                request_data.request_line_id = Guid.NewGuid();
                request_data.line = line_position;
                request_data.file = request;
                request_data.request_id = request.request_id;

                request_data.user_id = user_id;
                request_data.user_name = user_name;
                request_data.order_id = order_id;
                request_data.product_id = product_id;
                request_data.product_value = product_value;
                request_data.order_date = order_date;

                request.Lines.Add(request_data);
                await dataContext.SaveChangesAsync();
            }

            return Task.CompletedTask;
        }
    }
}
