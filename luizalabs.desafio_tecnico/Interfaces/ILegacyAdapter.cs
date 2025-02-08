namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface ILegacyAdapter
    {
        public List<Models.Legacy.LegacyView> ToListView(List<Models.Legacy.LegacyRequest> itens);

        public Models.Legacy.LegacyView ToView(Models.Legacy.LegacyRequest item);
    }
}
