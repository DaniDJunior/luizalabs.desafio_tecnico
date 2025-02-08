using luizalabs.desafio_tecnico.Interfaces;
using Newtonsoft.Json.Linq;

namespace luizalabs.desafio_tecnico.Adapters
{
    public class TokenAdapter : ITokenAdapter
    {
        public Models.Token.TokenView ToView(string token, string userName, string type, DateTime expires)
        {
            Models.Token.TokenView tokenReturn = new Models.Token.TokenView();
            tokenReturn.data = token;
            tokenReturn.type = type;
            tokenReturn.expires = expires;
            tokenReturn.username = userName;
            return tokenReturn;
        }
    }
}
