using Confluent.Kafka;
using luizalabs.desafio_tecnico.Data;
using luizalabs.desafio_tecnico.Enuns;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Kafka;
using Newtonsoft.Json;

namespace luizalabs.desafio_tecnico.Managers
{
    public class KafkaManager : IKafkaManager
    {
        private readonly ILogger<KafkaManager> Logger;
        private ProducerConfig Config { get; set; }

        private string bkp_patch { get; set; }

        public KafkaManager(ILogger<KafkaManager> logger, IConfiguration configuration)
        {
            Logger = logger;
            Config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };
        }

        public async Task<object> SendProcessFileAsync(string fileName, Guid requestId)
        {
            string message = JsonConvert.SerializeObject(new ProcessFile() { file_name = fileName, request_id = requestId });
            await SendPostAsync(KafkaTopics.LUIZALABS_PROCESS_FILE, message);
            return Task.CompletedTask;
        }

        public async Task<object> SendProcessLineAsync(string line, int linePosition, Guid requestId)
        {
            string message = JsonConvert.SerializeObject(new ProcessLine() { line = line, line_position = linePosition, request_id = requestId });
            await SendPostAsync(KafkaTopics.LUIZALABS_PROCESS_LINE, message);
            return Task.CompletedTask;
        }

        public async Task<object> SendProcessFinalAsync(Guid requestId)
        {
            string message = JsonConvert.SerializeObject(new ProcessFinal() { request_id = requestId });
            await SendPostAsync(KafkaTopics.LUIZALABS_PROCESS_FINAL, message);
            return Task.CompletedTask;
        }

        public async Task<object> SendProcessDataAsync(Guid requestLineId)
        {
            string message = JsonConvert.SerializeObject(new ProcessData() { request_line_id = requestLineId });
            await SendPostAsync(KafkaTopics.LUIZALABS_PROCESS_DATA, message);
            return Task.CompletedTask;
        }

        public async Task<DeliveryResult<Null, string>> SendPostAsync(string post, string message)
        {
            try
            {
                using (var producer = new ProducerBuilder<Null, string>(Config).Build())
                {
                    return await producer.ProduceAsync(post, new Message<Null, string> { Value = message });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Erro ao enviar a mensagem {post}: {ex.Message}");
                return null;
            }
        }
    }
}
