using luizalabs.desafio_tecnico.Data;

namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface ILegacyLogic
    {
        public Task<Models.Legacy.LegacyRequest> StartRequestAsync(string fileName, Guid id);

        public Task<object> ReprocessRequestAsync(Guid id, int? line);

        public Task<object> ProcessFileAsync(string fileName, Guid requestId);

        public Task<object> ProcessLineAsync(string line, int line_position, Guid requestId);

        public Task<object> ProcessFinalAsync(Guid requestId);

        public Task<object> ProcessDataAsync(Guid requestId);
    }
}
