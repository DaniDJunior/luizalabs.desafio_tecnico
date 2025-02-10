using luizalabs.desafio_tecnico.Models.Legacy;

namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface ILegacyData
    {
        public Task<List<Models.Legacy.LegacyRequest>> GetListAsync();

        public List<Models.Legacy.LegacyRequest> FindByFileName(string fileName);

        public Task<Models.Legacy.LegacyRequest?> GetByIdAsync(Guid id);

        public Task<Models.Legacy.LegacyRequestLine?> GetLineByIdAsync(Guid id);

        public Task<LegacyRequestLine?> GetLineByLineNumberAsync(Guid requestId, int lineNumber);

        public Task<Models.Legacy.LegacyRequest> AddErrorAsync(Models.Legacy.LegacyRequest request, int lineNumber, string message);

        public Task<Models.Legacy.LegacyRequest> AddWarningAsync(Models.Legacy.LegacyRequest request, int lineNumber, string message);

        public Task<Models.Legacy.LegacyRequest> LoadAsync(Models.Legacy.LegacyRequest request);

        public Task<Models.Legacy.LegacyRequest> SaveAsync(Models.Legacy.LegacyRequest request);

        public Task<Models.Legacy.LegacyRequest> AddLineAsync(Models.Legacy.LegacyRequest request, Models.Legacy.LegacyRequestLine requestLine);
    }
}
