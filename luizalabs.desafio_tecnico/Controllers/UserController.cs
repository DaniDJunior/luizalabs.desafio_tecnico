using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Filters;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Managers;
using luizalabs.desafio_tecnico.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IUserManager UserManager;
        private readonly IUserAdapter UserAdapter;

        public UserController(ILogger<UserController> logger, IUserAdapter userAdapter, IUserManager userManager)
        {
            Logger = logger;
            UserAdapter = userAdapter;
            UserManager = userManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            return StatusCode((int)HttpStatusCode.OK, UserAdapter.ToListView(await UserManager.GetListAsync(false)));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetId(Guid id)
        {
            var user = await UserManager.GetId(id, false);
            if (user == null) 
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.OK, UserAdapter.ToView(user));
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Post([FromBody]UserInsert model)
        {
            var user = UserAdapter.ToModel(model);
            user = await UserManager.Save(user);
            return StatusCode((int)HttpStatusCode.Created, UserAdapter.ToView(user));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put([FromBody] UserUpdate model, Guid id)
        {
            Models.User.User? user = await UserManager.GetId(id, false);
            if (user == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            user = UserAdapter.ToModel(model, user);
            user = await UserManager.Save(user);
            return StatusCode((int)HttpStatusCode.OK, UserAdapter.ToView(user));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Models.User.User? user = await UserManager.GetId(id, false);
            if (user == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            if (await UserManager.Delete(user))
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
