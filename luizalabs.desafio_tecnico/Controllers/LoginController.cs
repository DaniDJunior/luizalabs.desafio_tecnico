using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace luizalabs.desafio_tecnico.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Login")]
    public class LoginController : ControllerBase
    {
        private ILogger<LoginController> Logger;
        private ITokenLogic TokenManager { get; set; }
        private ITokenAdapter TokenAdapter { get; set; }

        public LoginController(ILogger<LoginController> logger, ITokenLogic tokenManager, ITokenAdapter tokenAdapter) { 
            Logger = logger;
            TokenManager = tokenManager;
            TokenAdapter = tokenAdapter;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> PostLogin()
        {

            string authHeader = HttpContext.Request.Headers["Authorization"];

            string[] authParts = authHeader.Split(' ');

            if (authParts[0].ToLower() == "basic")
            {
                byte[] data = Convert.FromBase64String(authParts[1]);
                string[] loginParts = System.Text.Encoding.UTF8.GetString(data).Split(';');
                string userName = loginParts[0];
                string password = string.Join(";", loginParts.Skip(1));

                if (!string.IsNullOrEmpty(userName))
                {
                    DateTime expires = DateTime.Now.AddDays(30);

                    string token = TokenManager.CreateToken(userName, expires);

                    return StatusCode((int)HttpStatusCode.OK, TokenAdapter.ToView(token, userName, "bearer", expires));
                }
            }
            return StatusCode((int)HttpStatusCode.Unauthorized, "Username ou password errados");

        }
    }
}
