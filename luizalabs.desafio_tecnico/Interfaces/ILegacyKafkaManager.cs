using luizalabs.desafio_tecnico.Data;

namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface ILegacyKafkaManager
    {
        public Task<object> ProcessLineAsync(string line, int line_position, Guid requestId);
    }
}
