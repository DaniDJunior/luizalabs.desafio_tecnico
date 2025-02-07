using Confluent.Kafka;
using luizalabs.desafio_tecnico.Enuns;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Kafka;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace luizalabs.desafio_tecnico.Services
{
    public class LegacyFileKafkaService : BackgroundService
    {
        private readonly IConsumer<Ignore, string>? Consumer;

        private readonly ILogger<LegacyFileKafkaService> Logger;

        private readonly IKafkaManager KafkaManager;

        public LegacyFileKafkaService(IConfiguration configuration, ILogger<LegacyFileKafkaService> logger, IKafkaManager kafkaManager)
        {
            Logger = logger;
            KafkaManager = kafkaManager;

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = configuration["Kafka:GroupName"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            try
            {
                Consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            }
            catch (Exception ex)
            {
                Consumer = null;
                Logger.LogError($"Erro ao iniciar o consumo do Kafka: {ex.Message}");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            if (Consumer != null)
            {
                try
                {
                    Consumer.Subscribe(KafkaTopics.LUIZALABS_PROCESS_FILE);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await ProcessKafkaMessageAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Erro ao consumir a mensagem do topico {KafkaTopics.LUIZALABS_PROCESS_FILE}: {ex.Message}");
                }
                finally
                {
                    Consumer.Close();
                }
            }
        }

        public async Task<object> ProcessKafkaMessageAsync(CancellationToken stoppingToken)
        {
            if (Consumer != null)
            {
                try
                {
                    var consumeResult = Consumer.Consume(stoppingToken);

                    var message = consumeResult.Message.Value;

                    var returnmsg = string.Empty;

                    if (!string.IsNullOrEmpty(message))
                    {
                        returnmsg = await TopicProcessFile(message);
                    }

                    Logger.LogInformation($"Received inventory update: {returnmsg}");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error processing Kafka message: {ex.Message}");
                }
            }
            return Task.CompletedTask;
        }

        private async Task<string> TopicProcessFile(string message)
        {
            ProcessFile? process = JsonConvert.DeserializeObject<ProcessFile>(message);
            if (process == null) { return string.Empty; }

            string[] lines = await System.IO.File.ReadAllLinesAsync(process.file_name);

            for (int i = 0; i < lines.Length; i++)
            {
                await KafkaManager.SendProcessLineAsync(lines[i], i, process.request_id);
            }

            return message;
        }

    }
}
