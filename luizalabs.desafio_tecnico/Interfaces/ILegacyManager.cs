namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface ILegacyManager
    {
        public Task<List<Models.Legacy.LegacyRequest>> GetListAsync();

        public List<Models.Legacy.LegacyRequest> FindByFileName(string fileName);

        public Task<Models.Legacy.LegacyRequest?> GetAsync(Guid id);

        public Models.Legacy.LegacyRequest Load(Models.Legacy.LegacyRequest request);

        public Task<Models.Legacy.LegacyRequest> StartRequestAsync(string fileName, Guid id);
    }
}
