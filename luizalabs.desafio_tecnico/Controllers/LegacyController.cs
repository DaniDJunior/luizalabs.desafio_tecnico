using luizalabs.desafio_tecnico.Filters;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Logics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;

namespace luizalabs.desafio_tecnico.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [AuthFilter]
    [Route("api/v{version:apiVersion}/Legacy")]
    public class LegacyController : ControllerBase
    {
        private readonly ILogger<LegacyController> Logger;
        private readonly ILegacyData LegacyData;
        private readonly ILegacyLogic LegacyLogic;
        private readonly ILegacyAdapter LegacyAdapter;
        private readonly string BkpPatch;

        public LegacyController(ILogger<LegacyController> logger, IConfiguration configuration, ILegacyAdapter legacyAdapter, ILegacyLogic legacyLogic, ILegacyData legacyData)
        {
            Logger = logger;
            string? bkp_patch = configuration["Legacy:Bkp_Patch"];
            BkpPatch = bkp_patch == null ? string.Empty : bkp_patch;
            LegacyAdapter = legacyAdapter;
            LegacyLogic = legacyLogic;
            LegacyData = legacyData;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(string? fileName)
        {
            if (fileName == null)
            {
                return StatusCode((int)HttpStatusCode.OK, LegacyAdapter.ToListView(await LegacyData.GetListAsync()));
            }
            else
            {
                return StatusCode((int)HttpStatusCode.OK, LegacyAdapter.ToListView(LegacyData.FindByFileName(fileName)));
            }
        }

        [HttpGet]
        [Route("{request_id}")]
        public async Task<IActionResult> GetId(Guid request_id)
        {
            var request = await LegacyData.GetByIdAsync(request_id);
            if (request != null)
            {
                return StatusCode((int)HttpStatusCode.OK, LegacyAdapter.ToView(request));
            }
            else
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("{request_id}/reprocess/{line}")]
        public async Task<IActionResult> Reprocess(Guid request_id, int line)
        {
            await LegacyLogic.ReprocessRequestAsync(request_id, line);
            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("{request_id}/reprocess")]
        public async Task<IActionResult> ReprocessAll(Guid request_id)
        {
            await LegacyLogic.ReprocessRequestAsync(request_id, null);
            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Start([FromForm] IFormFile file)
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
                    
                    Models.Legacy.LegacyRequest legacyFile = await LegacyLogic.StartRequestAsync(file.FileName, id);

                    return StatusCode((int)HttpStatusCode.Created, LegacyAdapter.ToView(legacyFile));
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
