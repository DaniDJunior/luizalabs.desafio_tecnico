namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface ILegacyManager
    {
        public Task<List<Models.Legacy.LegacyFile>> GetListAsync();

        public Task<Models.Legacy.LegacyFile> StartRequestAsync(string fileName, Guid id);
    }
}
