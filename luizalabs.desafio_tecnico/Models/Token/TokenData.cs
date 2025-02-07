namespace luizalabs.desafio_tecnico.Models.Token
{
    public class TokenData
    {
        public bool ok { get; set; }
        public bool valid { get; set; }
        public DateTime Expiration { get; set; }
        public string User { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public List<string> Roles { get; set; }
    }
}
