﻿using Confluent.Kafka;
using Confluent.Kafka.Admin;
using luizalabs.desafio_tecnico.Enuns;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Managers;
using luizalabs.desafio_tecnico.Models.Kafka;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace luizalabs.desafio_tecnico.Services
{
    public class LegacyFileKafkaService : BackgroundService
    {
        private readonly IConsumer<Ignore, string>? Consumer;

        private readonly ILogger<LegacyFileKafkaService> Logger;

        private ILegacyLogic LegacyKafkaManager;

        private string BootstrapServers;

        public LegacyFileKafkaService(IConfiguration configuration, ILogger<LegacyFileKafkaService> logger, ILegacyLogic legacyKafkaManager)
        {
            Logger = logger;
            LegacyKafkaManager = legacyKafkaManager;

            string? bootstrapServer = configuration["Kafka:BootstrapServers"];
            BootstrapServers = bootstrapServer != null ? bootstrapServer : string.Empty;

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

            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = BootstrapServers }).Build())
            {
                try
                {
                    await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                    new TopicSpecification { Name = KafkaTopics.LUIZALABS_PROCESS_FILE, ReplicationFactor = 1, NumPartitions = 1 } });
                }
                catch (CreateTopicsException e)
                {
                    Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
                }
            }

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

            await LegacyKafkaManager.ProcessFileAsync(process.file_name, process.request_id);

            return message;
        }

    }
}
