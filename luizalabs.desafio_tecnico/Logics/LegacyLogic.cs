using Confluent.Kafka;
using luizalabs.desafio_tecnico.Data;
using luizalabs.desafio_tecnico.Enuns;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Order;
using luizalabs.desafio_tecnico.Models.User;
using luizalabs.desafio_tecnico.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Diagnostics;
using System.Globalization;

namespace luizalabs.desafio_tecnico.Logics
{
    public class LegacyLogic : ILegacyLogic
    {
        private readonly ILogger<LegacyLogic> Logger;
        private readonly IKafkaCommunication KafkaManager;
        private readonly ILegacyData LegacyData;
        private readonly IUserData UserData;
        private readonly IOrderData OrderData;
        private readonly string BkpPatch;

        public LegacyLogic(ILogger<LegacyLogic> logger, IConfiguration configuration, IKafkaCommunication kafkaManager, ILegacyData legacyData, IUserData userData, IOrderData orderData)
        {
            Logger = logger;
            string? bkp_patch = configuration["Legacy:Bkp_Patch"];
            BkpPatch = bkp_patch == null ? string.Empty : bkp_patch;
            KafkaManager = kafkaManager;
            LegacyData = legacyData;
            UserData = userData;
            OrderData = orderData;
        }

        public async Task<Models.Legacy.LegacyRequest> StartRequestAsync(string fileName, Guid id)
        {
            int lines_count = (await File.ReadAllLinesAsync(BkpPatch + id.ToString())).Length;

            Models.Legacy.LegacyRequest request = new Models.Legacy.LegacyRequest();
            request.request_id = id;
            request.total_lines = lines_count;
            request.file_name = fileName;
            request.status = LegacyFileStatus.RECEIVED;
            request.lines = new List<Models.Legacy.LegacyRequestLine>();

            await LegacyData.SaveAsync(request);

            await KafkaManager.SendProcessFileAsync(BkpPatch + id.ToString(), request.request_id);

            return request;
        }

        public async Task<object> ReprocessRequestAsync(Guid id, int? line)
        {
            if(line == null)
            {
                await KafkaManager.SendProcessFileAsync(BkpPatch + id.ToString(), id);
            }
            else
            {
                Models.Legacy.LegacyRequestLine? requestLine = await LegacyData.GetLineByLineNumberAsync(id, line.Value);
                if (requestLine != null) 
                {
                    await KafkaManager.SendProcessDataAsync(requestLine.request_line_id);
                }
            }
            return Task.CompletedTask;
        }

        public async Task<object> ProcessFileAsync(string fileName, Guid requestId)
        {
            Models.Legacy.LegacyRequest? request = await LegacyData.GetByIdAsync(requestId);

            if (request == null)
            {
                return Task.CompletedTask;
            }

            request.status = LegacyFileStatus.PROCESSED;

            await LegacyData.SaveAsync(request);

            string[] lines = await File.ReadAllLinesAsync(fileName);

            for (int i = 0; i < lines.Length; i++)
            {
                await KafkaManager.SendProcessLineAsync(lines[i], i, requestId);
            }

            return Task.CompletedTask;
        }

        private async Task<object> CheckProcessFinal(Models.Legacy.LegacyRequest request)
        {
            int lines_process = request.lines.Count;

            if (request.total_lines == lines_process)
            {
                await KafkaManager.SendProcessFinalAsync(request.request_id);
            }

            return Task.CompletedTask;
        }

        public async Task<object> ProcessLineAsync(string line, int linePosition, Guid requestId)
        {
            Models.Legacy.LegacyRequest? request = await LegacyData.GetByIdAsync(requestId);

            if (request == null)
            {
                return Task.CompletedTask;
            }

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
                string message = $"Erro na conversão do ID de usuário - {line_user_id} - Linha {linePosition}";
                await LegacyData.AddErrorAsync(request, linePosition, message);
                Logger.LogError(message);
                return Task.CompletedTask;
            }

            if(user_id <= 0)
            {
                string message = $"O ID de usuário {user_id} não é válido - Linha {linePosition}";
                await LegacyData.AddErrorAsync(request, linePosition, message);
                Logger.LogError(message);
                return Task.CompletedTask;
            }

            user_name = line_user_name.Trim();

            if (!int.TryParse(line_order_id, out order_id))
            {
                string message = $"Erro na conversão do ID da ordem - {line_order_id} - Linha {linePosition}";
                await LegacyData.AddErrorAsync(request, linePosition, message);
                Logger.LogError(message);
                return Task.CompletedTask;
            }

            if (order_id <= 0)
            {
                string message = $"O ID da order {order_id} não é válido - Linha {linePosition}";
                await LegacyData.AddErrorAsync(request, linePosition, message);
                Logger.LogError(message);
                return Task.CompletedTask;
            }

            if (!int.TryParse(line_product_id, out product_id))
            {
                string message = $"Erro na conversão do ID do produto - {line_product_id} - Linha {linePosition}";
                await LegacyData.AddErrorAsync(request, linePosition, message);
                Logger.LogError(message);
                return Task.CompletedTask;
            }

            if (product_id <= 0)
            {
                string message = $"O ID do produto {product_id} não é válido - Linha {linePosition}";
                await LegacyData.AddErrorAsync(request, linePosition, message);
                Logger.LogError(message);
                return Task.CompletedTask;
            }

            if (!float.TryParse(line_product_value.Trim(), out product_value))
            {
                string message = $"Erro na conversão do valor do produto - {line_product_value} - Linha {linePosition}";
                await LegacyData.AddErrorAsync(request, linePosition, message);
                Logger.LogError(message);
                return Task.CompletedTask;
            }

            if (!DateTime.TryParseExact(line_order_date, "yyyyMMdd", null, DateTimeStyles.None, out order_date))
            {
                string message = $"Erro na conversão da data da ordem - {line_order_date} - Linha {linePosition}";
                await LegacyData.AddErrorAsync(request, linePosition, message);
                Logger.LogError(message);
                return Task.CompletedTask;
            }

            Models.Legacy.LegacyRequestLine? requestLine = request.lines.FirstOrDefault(rl => rl.line_number == linePosition);

            if (requestLine == null)
            {
                requestLine = new Models.Legacy.LegacyRequestLine();
            }

            requestLine.line_number = linePosition;
            requestLine.request_id = request.request_id;

            requestLine.user_id = user_id;
            requestLine.user_name = user_name;
            requestLine.order_id = order_id;
            requestLine.product_id = product_id;
            requestLine.product_value = product_value;
            requestLine.order_date = order_date;

            request = await LegacyData.AddLineAsync(request, requestLine);

            await KafkaManager.SendProcessDataAsync(requestLine.request_line_id);

            await CheckProcessFinal(request);

            return Task.CompletedTask;
        }

        public async Task<object> ProcessFinalAsync(Guid requestId)
        {
            Models.Legacy.LegacyRequest? request = await LegacyData.GetByIdAsync(requestId);

            if (request == null)
            {
                return Task.CompletedTask;
            }

            if(request.errors.Count > 0)
            {
                if(request.errors.Max(e=>e.level) == 2)
                {
                    request.status = LegacyFileStatus.COMPLETE_WITH_ERROR;
                }
                else
                {
                    request.status = LegacyFileStatus.COMPLETE_WITH_WARNING;
                }
                
            }
            else
            {
                request.status = LegacyFileStatus.COMPLETE;
            }

            ///TODO: postback

            await LegacyData.SaveAsync(request);

            return Task.CompletedTask;
        }

        public async Task<object> ProcessDataAsync(Guid requestLineId)
        {
            Models.Legacy.LegacyRequestLine? requestLine = await LegacyData.GetLineByIdAsync(requestLineId);

            if (requestLine == null)
            {
                return Task.CompletedTask;
            }

            Models.User.User? user = await UserData.GetByIdLegacyAsync(requestLine.user_id);

            if (user == null)
            {
                try
                {
                    user = new Models.User.User();

                    user.legacy_user_id = requestLine.user_id;
                    user.name = requestLine.user_name;

                    user = await UserData.SaveAsync(user);
                }
                catch (Exception ex)
                {
                    string message = $"Erro ao salvar o user {requestLine.user_id} - linha {requestLine.line_number} - requisição {requestLine.request_id}";
                    Logger.LogError(message, ex);
                    await LegacyData.AddErrorAsync(requestLine.request, requestLine.line_number, message);
                    return Task.CompletedTask;
                }
        }
            else
            {
                if (user.name != requestLine.user_name)
                {
                    await LegacyData.AddWarningAsync(requestLine.request, requestLine.line_number, $"O usuário da linha {requestLine.line_number} de user_id {user.legacy_user_id} e nome {requestLine.user_name} já existe na base com o nome {user.name}");
                }
            }

            Models.Order.Order? order = await OrderData.GetByIdLegacyAsync(requestLine.order_id);

            if (order == null)
            {
                try
                {
                    order = new Models.Order.Order();
                    order.legacy_order_id = requestLine.order_id;
                    order.date = requestLine.order_date;
                    order.user_id = user.user_id;
                    order.user = user;

                    await OrderData.SaveAsync(order);
                }
                catch (Exception ex)
                {
                    
                    string message = $"Erro ao salvar a order {requestLine.order_id} - user {requestLine.user_id} - linha {requestLine.line_number} - requisição {requestLine.request_id}";
                    Logger.LogError(message, ex);
                    await LegacyData.AddErrorAsync(requestLine.request, requestLine.line_number, message);
                    return Task.CompletedTask;
                }
            }
            else
            {
                if (order.date != requestLine.order_date)
                {
                    await LegacyData.AddWarningAsync(requestLine.request, requestLine.line_number, $"A order da linha {requestLine.line_number} de order_id {order.legacy_order_id} com a data {requestLine.order_date} já existe na base com a seguinte data {order.date}");
                }
            }

            OrderProduct? product = order.products.FirstOrDefault(op => op.legacy_product_id == requestLine.product_id);

            if (product == null)
            {
                try
                {
                    product = new OrderProduct();
                    product.legacy_product_id = requestLine.product_id;
                    product.value = requestLine.product_value;
                    product.order = order;
                    product.order_id = order.order_id;

                    order = await OrderData.AddProductAsync(order, product);
                }
                catch (Exception ex)
                {
                    string message = $"Erro ao salvar o produto - {requestLine.product_id} - order {requestLine.order_id} - user {requestLine.user_id} - linha {requestLine.line_number} - requisição {requestLine.request_id}";
                    Logger.LogError(message, ex);
                    await LegacyData.AddErrorAsync(requestLine.request, requestLine.line_number, message);
                    return Task.CompletedTask;
                }
            }
            else
            {
                await LegacyData.AddWarningAsync(requestLine.request, requestLine.line_number, $"O produto da linha {requestLine.line_number} da order_id {order.order_id} e product_id {product.legacy_product_id} de valor {requestLine.product_value}, já foi adicionado com o seguinte valor {product.value}");
            }

            return Task.CompletedTask;
        }
    }
}
