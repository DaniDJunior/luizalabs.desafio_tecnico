using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Legacy;

namespace luizalabs.desafio_tecnico.Adapters
{
    public class LegacyAdapter : ILegacyAdapter
    {
        public List<LegacyOut> FromListModel(List<LegacyFile> itens)
        {
            return itens.Select(item => FromModel(item)).ToList();
        }

        public LegacyOut FromModel(LegacyFile item)
        {
            LegacyOut legacy = new LegacyOut();

            legacy.request_id = item.request_id;
            legacy.total_lines = item.total_lines;
            legacy.status = item.status;
            legacy.file_name = item.file_name;
            legacy.processed_lines = item.Lines.Count;

            return legacy;
        }
    }
}
