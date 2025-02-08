namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface ITokenAdapter
    {
        public Models.Token.TokenView ToView(string token, string userName, string type, DateTime expires);
    }
}
