namespace luizalabs.desafio_tecnico.Models.Legacy
{
    public class LegacyView
    {
        public Guid request_id { get; set; }
        public string file_name { get; set; }
        public int total_lines { get; set; }
        public string status { get; set; }
        public float processed_lines { get; set; }
        public List<LegacyErrorView> errors { get; set; }

        public LegacyView()
        {
            file_name = string.Empty;
            status = string.Empty;
            errors = new List<LegacyErrorView>();
        }
    }
}
