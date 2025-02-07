using Microsoft.AspNetCore.Mvc.Filters;

namespace luizalabs.desafio_tecnico.Interfaces
{
    public interface IAuthManager
    {
        public Task<bool> ValidBearerAuth(string authHeader, List<string> roles, ActionExecutingContext context);
    }
}
