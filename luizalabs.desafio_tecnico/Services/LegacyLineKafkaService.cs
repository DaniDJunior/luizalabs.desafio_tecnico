using Confluent.Kafka;
using luizalabs.desafio_tecnico.Enuns;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Kafka;
using Newtonsoft.Json;

namespace luizalabs.desafio_tecnico.Services
{
    public class LegacyLineKafkaService : BackgroundService
    {
        private readonly IConsumer<Ignore, string> Consumer;

        private readonly ILogger<LegacyLineKafkaService> Logger;

        private ILegacyKafkaManager LegacyFileManager;

        public LegacyLineKafkaService(IConfiguration configuration, ILogger<LegacyLineKafkaService> logger, ILegacyKafkaManager legacyFileManager)
        {
            Logger = logger;
            LegacyFileManager = legacyFileManager;

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
                    Consumer.Subscribe(KafkaTopics.LUIZALABS_PROCESS_LINE);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await ProcessKafkaMessageAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Erro ao consumir a mensagem do topico {KafkaTopics.LUIZALABS_PROCESS_LINE}: {ex.Message}");
                }
                finally
                {
                    Consumer.Close();
                }
            }
        }

        public async Task<object> ProcessKafkaMessageAsync(CancellationToken stoppingToken)
        {
            try
            {
                var consumeResult = Consumer.Consume(stoppingToken);

                var message = consumeResult.Message.Value;

                var returnmsg = string.Empty;

                if (!string.IsNullOrEmpty(message))
                {
                    returnmsg = await TopicProcessLine(message);
                }

                Logger.LogInformation($"Received inventory update: {returnmsg}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error processing Kafka message: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        private async Task<string> TopicProcessLine(string message)
        {
            ProcessLine? process = JsonConvert.DeserializeObject<ProcessLine>(message);

            if (process == null) { return string.Empty; }

            await LegacyFileManager.ProcessLineAsync(process.line, process.line_position, process.request_id);

            return message;
        }
    }
}
