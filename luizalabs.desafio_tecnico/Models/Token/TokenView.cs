namespace luizalabs.desafio_tecnico.Models.Token
{
    public class TokenView
    {
        public string data { get; set; }
        public string type { get; set; }
        public string username { get; set; }
        public DateTime expires { get; set; }

        public TokenView() 
        { 
            username = string.Empty;
            data = string.Empty;
            type = string.Empty;
        }
    }
}
