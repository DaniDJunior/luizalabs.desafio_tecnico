using luizalabs.desafio_tecnico.Models.Token;

namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface ITokenLogic
    {
        public string getPublicKey();
        public string CreateToken(string user, DateTime expires);
        public Token ValidateToken(string token);
        public string Encrypt(string keyString, string dataToEncrypt);
    }
}
