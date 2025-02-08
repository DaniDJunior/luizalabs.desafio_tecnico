using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Legacy;

namespace luizalabs.desafio_tecnico.Adapters
{
    public class LegacyAdapter : ILegacyAdapter
    {
        public List<LegacyView> ToListView(List<LegacyRequest> itens)
        {
            return itens.Select(item => ToView(item)).ToList();
        }

        public LegacyView ToView(LegacyRequest item)
        {
            LegacyView legacy = new LegacyView();

            legacy.request_id = item.request_id;
            legacy.total_lines = item.total_lines;
            legacy.status = item.status;
            legacy.file_name = item.file_name;
            legacy.processed_lines = item.total_lines != 0 ? (float)item.Lines.Count / (float)item.total_lines : 0;

            return legacy;
        }
    }
}
