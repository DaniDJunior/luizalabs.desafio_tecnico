namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface ILegacyAdapter
    {
        public List<Models.Legacy.LegacyOut> FromListModel(List<Models.Legacy.LegacyFile> itens);

        public Models.Legacy.LegacyOut FromModel(Models.Legacy.LegacyFile item);
    }
}
