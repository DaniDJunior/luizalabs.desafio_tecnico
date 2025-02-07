using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using luizalabs.desafio_tecnico.Interfaces;

namespace luizalabs.desafio_tecnico.Filters
{
    public class AuthParameter
    {
        public List<string> Regras { get; set; }

        public AuthParameter(string regras)
        {
            Regras = new List<string>(regras.Split(','));
        }
    }

    public class AuthFilter : TypeFilterAttribute
    {
        public AuthFilter() : base(typeof(AuthFilterImpl))
        {
            Arguments = new object[] { new AuthParameter(string.Empty) };
        }

        public AuthFilter(string regras) : base(typeof(AuthFilterImpl))
        {
            Arguments = new object[] { new AuthParameter(regras) };
        }

        public class AuthFilterImpl : IAsyncActionFilter
        {
            List<string> Regras { get; set; }

            private IAuthManager AuthManager;

            public AuthFilterImpl(AuthParameter parameter, IAuthManager authManager)
            {
                Regras = parameter.Regras;
                AuthManager = authManager;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                string authHeader = context.HttpContext.Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authHeader))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
                else
                {
                    string[] authHeaderParts = authHeader.Split(' ');
                    if (authHeader.Length >= 2)
                    {

                        switch (authHeaderParts[0].ToLower())
                        {
                            case "bearer":
                                if (!await AuthManager.ValidBearerAuth(authHeaderParts[1], Regras, context))
                                {
                                    var result_regra = new ObjectResult("O token não foi validado ou vc não tem permissão");
                                    result_regra.StatusCode = 401;
                                    context.Result = result_regra;
                                    return;
                                }
                                break;
                            default:
                                var result = new ObjectResult("O cabeçalho não está no formato correto");
                                result.StatusCode = 401;
                                context.Result = result;
                                return;
                        }
                        await next();
                    }
                    else
                    {
                        var result = new ObjectResult("A requisição não tem cabeçalho");
                        result.StatusCode = 401;
                        context.Result = result;
                        return;
                    }
                }
            }
        }
    }
}
