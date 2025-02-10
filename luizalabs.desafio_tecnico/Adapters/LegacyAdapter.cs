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
            legacy.processed_lines = item.total_lines != 0 ? (float)item.lines.Count / (float)item.total_lines : 0;
            legacy.errors = ToErrorListView(item.errors.ToList());

            return legacy;
        }

        public List<LegacyErrorView> ToErrorListView(List<LegacyRequestError> itensErrors)
        {
            return itensErrors.Select(itemError => ToErrorView(itemError)).ToList();
        }

        private LegacyErrorView ToErrorView(LegacyRequestError itemError)
        { 
            LegacyErrorView legacyErrorView = new LegacyErrorView();

            switch (itemError.level)
            {
                case 1:
                    legacyErrorView.level = "Warning";
                    break;
                case 2:
                    legacyErrorView.level = "Error";
                    break;
                default:
                    legacyErrorView.level = "Order";
                    break;
            }
            legacyErrorView.message = itemError.message;
            legacyErrorView.line_number = itemError.line_number;

            return legacyErrorView;
        }
    }
}
