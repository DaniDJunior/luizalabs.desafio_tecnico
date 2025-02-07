﻿using Confluent.Kafka;
using luizalabs.desafio_tecnico.Data;

namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface IKafkaManager
    {
        public Task<DeliveryResult<Null, string>> SendPostAsync(string post, string message);

        public Task<object> SendProcessLineAsync(string line, int linePosition, Guid requestId);

        public Task<object> SendProcessFileAsync(string fileName, Guid requestId);
    }
}
