using Confluent.Kafka;
using luizalabs.desafio_tecnico.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luizalabs.desafio_tecnico.test.Communications
{
    internal class KafkaCommunication : IKafkaCommunication
    {
        public async Task<DeliveryResult<Null, string>> SendPostAsync(string post, string message)
        {
            Console.WriteLine($"Kafka: {post}");
            return null;
        }

        public async Task<object> SendProcessDataAsync(Guid requestLineId)
        {
            Console.WriteLine($"Kafka: ProcessData");

            return Task.CompletedTask;
        }

        public async Task<object> SendProcessFileAsync(string fileName, Guid requestId)
        {
            Console.WriteLine($"Kafka: ProcessFile");

            return Task.CompletedTask;
        }

        public async Task<object> SendProcessFinalAsync(Guid requestId)
        {
            Console.WriteLine($"Kafka: ProcessFinal");

            return Task.CompletedTask;
        }

        public async Task<object> SendProcessLineAsync(string line, int linePosition, Guid requestId)
        {
            Console.WriteLine($"Kafka: ProcessLine");

            return Task.CompletedTask;
        }
    }
}
