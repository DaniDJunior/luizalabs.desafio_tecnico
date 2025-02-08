namespace luizalabs.desafio_tecnico.Models.Kafka
{
    public class ProcessLine
    {
        public string line { get; set; }
        public int line_position { get; set; }
        public Guid request_id { get; set; }

        public ProcessLine()
        {
            line = string.Empty;
        }
    }
}
