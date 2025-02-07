namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface ITokenAdapter
    {
        public Models.Token.TokenOut FromModel(string token, string userName, string type, DateTime expires);
    }
}
