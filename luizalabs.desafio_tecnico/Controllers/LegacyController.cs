using luizalabs.desafio_tecnico.Filters;
using luizalabs.desafio_tecnico.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace luizalabs.desafio_tecnico.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Legacy")]
    public class LegacyController : ControllerBase
    {
        private readonly ILogger<LegacyController> Logger;
        private readonly ILegacyManager LegacyManager;
        private readonly ILegacyAdapter LegacyAdapter;
        private readonly string BkpPatch;

        public LegacyController(ILogger<LegacyController> logger, IConfiguration configuration, ILegacyAdapter legacyAdapter, ILegacyManager legacyManager)
        {
            Logger = logger;
            BkpPatch = configuration["Legacy:Bkp_Patch"];
            LegacyAdapter = legacyAdapter;
            LegacyManager = legacyManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            return StatusCode((int)HttpStatusCode.OK, LegacyAdapter.FromListModel(await LegacyManager.GetListAsync()));
        }

        [HttpPost]
        [AuthFilter]
        [Route("")]
        public async Task<IActionResult> Update([FromForm] IFormFile file)
        {
            if (file.Length > 0)
            {
                try
                {
                    Guid id = Guid.NewGuid();

                    if (!Directory.Exists(BkpPatch))
                    {
                        Directory.CreateDirectory(BkpPatch);
                    }
                    using (FileStream filestream = System.IO.File.Create(BkpPatch + id.ToString()))
                    {
                        await file.CopyToAsync(filestream);
                        filestream.Flush();
                    }
                    
                    Models.Legacy.LegacyFile legacyFile = await LegacyManager.StartRequestAsync(file.FileName, id);

                    return StatusCode((int)HttpStatusCode.OK, LegacyAdapter.FromModel(legacyFile));
                }
                catch (Exception ex)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, ex.ToString());
                }
            }
            else
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu uma falha no envio do arquivo...");
            }

        }
    }
}
