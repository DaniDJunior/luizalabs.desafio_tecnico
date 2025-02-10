using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Data;
using luizalabs.desafio_tecnico.Filters;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Managers;
using luizalabs.desafio_tecnico.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Globalization;
using System.Net;

namespace luizalabs.desafio_tecnico.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [AuthFilter]
    [Route("api/v{version:apiVersion}/User")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> Logger;
        private readonly IUserData UserData;
        private readonly IUserAdapter UserAdapter;

        public UserController(ILogger<UserController> logger, IUserAdapter userAdapter, IUserData userData)
        {
            Logger = logger;
            UserAdapter = userAdapter;
            UserData = userData;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            return StatusCode((int)HttpStatusCode.OK, UserAdapter.ToListView(await UserData.GetListAsync()));
        }

        [HttpGet]
        [Route("{user_id}")]
        public async Task<IActionResult> GetId(Guid user_id)
        {
            var user = await UserData.GetByIdAsync(user_id);
            if (user == null) 
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.OK, UserAdapter.ToView(user));
            }
        }

        [HttpGet]
        [Route("legacy/{request_id}")]
        public async Task<IActionResult> GetRequestId(Guid request_id, string? user_name, string? datemin, string? datemax)
        {
            DateTime? date_datemin = null;
            DateTime? date_datemax = null;

            if(datemin != null)
            {
                DateTime temp_datemin;
                if (!DateTime.TryParseExact(datemin, "yyyyMMdd", null, DateTimeStyles.None, out temp_datemin))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest,"Data deve estar no padrão yyyymmdd");
                }
                else
                {
                    date_datemin = temp_datemin;
                }
            }

            if (datemax != null)
            {
                DateTime temp_datemax;
                if (!DateTime.TryParseExact(datemax, "yyyyMMdd", null, DateTimeStyles.None, out temp_datemax))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "Data deve estar no padrão yyyymmdd");
                }
                else
                {
                    date_datemax = temp_datemax;
                }
            }

            var users = await UserData.GetListByRequestIdAsync(request_id, user_name, date_datemin, date_datemax);
            if (users == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.OK, UserAdapter.ToListViewLegacy(users));
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Post([FromBody]UserInsert model)
        {
            var user = UserAdapter.ToModel(model);
            user = await UserData.SaveAsync(user);
            return StatusCode((int)HttpStatusCode.Created, UserAdapter.ToView(user));
        }

        [HttpPut]
        [Route("{user_id}")]
        public async Task<IActionResult> Put([FromBody] UserUpdate model, Guid user_id)
        {
            Models.User.User? user = await UserData.GetByIdAsync(user_id);
            if (user == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            user = UserAdapter.ToModel(model, user);
            user = await UserData.SaveAsync(user);
            return StatusCode((int)HttpStatusCode.OK, UserAdapter.ToView(user));
        }

        [HttpDelete]
        [Route("{user_id}")]
        public async Task<IActionResult> Delete(Guid user_id)
        {
            Models.User.User? user = await UserData.GetByIdAsync(user_id);
            if (user == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            if (await UserData.DeleteAsync(user))
            {
                return StatusCode((int)HttpStatusCode.OK);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
