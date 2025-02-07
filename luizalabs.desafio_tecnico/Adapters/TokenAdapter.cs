using luizalabs.desafio_tecnico.Interfaces;
using Newtonsoft.Json.Linq;

namespace luizalabs.desafio_tecnico.Adapters
{
    public class TokenAdapter : ITokenAdapter
    {
        public Models.Token.TokenOut FromModel(string token, string userName, string type, DateTime expires)
        {
            Models.Token.TokenOut tokenReturn = new Models.Token.TokenOut();
            tokenReturn.data = token;
            tokenReturn.type = type;
            tokenReturn.expires = expires;
            tokenReturn.username = userName;
            return tokenReturn;
        }
    }
}
