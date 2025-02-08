using luizalabs.desafio_tecnico.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace luizalabs.desafio_tecnico.Managers
{
    public class AuthManager : IAuthManager
    {
        private ITokenManager TokenManager;

        public AuthManager(ITokenManager managerToken)
        {
            TokenManager = managerToken;
        }

        public async Task<bool> ValidBearerAuth(string authHeader, List<string> roles, ActionExecutingContext context)
        {
            Models.Token.Token token = TokenManager.ValidateToken(authHeader);
            if (!token.valid)
            {
                return false;
            }
            var controller = (ControllerBase)context.Controller;
            controller.HttpContext.Items.Add("UserName", JsonConvert.SerializeObject(token.User));
            controller.HttpContext.Items.Add("Token", JsonConvert.SerializeObject(token));
            return token.valid;
        }
    }
}
