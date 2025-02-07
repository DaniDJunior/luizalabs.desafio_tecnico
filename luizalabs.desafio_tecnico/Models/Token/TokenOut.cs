namespace luizalabs.desafio_tecnico.Models.Token
{
    public class TokenOut
    {
        public string data { get; set; }
        public string type { get; set; }
        public string username { get; set; }
        public DateTime expires { get; set; }
    }
}
