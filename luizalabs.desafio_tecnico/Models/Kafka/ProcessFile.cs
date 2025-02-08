namespace luizalabs.desafio_tecnico.Models.Kafka
{
    public class ProcessFile
    {
        public string file_name { get; set; }
        public Guid request_id { get; set; }

        public ProcessFile()
        {
            file_name = string.Empty;
        }
    }
}
