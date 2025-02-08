using luizalabs.desafio_tecnico.Controllers;
using luizalabs.desafio_tecnico.Data;
using luizalabs.desafio_tecnico.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Globalization;

namespace luizalabs.desafio_tecnico.Managers
{
    public class LegacyManager : ILegacyManager
    {
        private readonly ILogger<LegacyManager> Logger;
        private readonly IKafkaManager KafkaManager;
        private readonly DataContext DataContext;
        private readonly string? BkpPatch;

        public LegacyManager(ILogger<LegacyManager> logger, IConfiguration configuration, DataContext dataContext, IKafkaManager kafkaManager) 
        {
            Logger = logger;
            BkpPatch = configuration["Legacy:Bkp_Patch"];
            DataContext = dataContext;
            KafkaManager = kafkaManager;
        }

        public async Task<Models.Legacy.LegacyRequest?> GetAsync(Guid id)
        {
            Models.Legacy.LegacyRequest? request = await DataContext.Requests.FirstOrDefaultAsync(request => request.request_id == id);
            if (request != null)
            {
                Load(request);
            }
            return request;
        }

        public async Task<List<Models.Legacy.LegacyRequest>> GetListAsync()
        {
            List<Models.Legacy.LegacyRequest> requests = await DataContext.Requests.ToListAsync();
            requests.ForEach(request => Load(request));
            return requests;
        }

        public List<Models.Legacy.LegacyRequest> FindByFileName(string fileName)
        {
            List<Models.Legacy.LegacyRequest> requests = DataContext.Requests.Where(request => request.file_name == fileName).ToList();
            requests.ForEach(request => Load(request));
            return requests;
        }

        public Models.Legacy.LegacyRequest Load(Models.Legacy.LegacyRequest request)
        {
            request.Lines = DataContext.RequestsLines.Where(line => line.request_id == request.request_id).ToList();
            return request;
        }

        public async Task<Models.Legacy.LegacyRequest> StartRequestAsync(string fileName, Guid id)
        {
            int lines_count = (await System.IO.File.ReadAllLinesAsync(BkpPatch + id.ToString())).Length;

            Models.Legacy.LegacyRequest request = new Models.Legacy.LegacyRequest();
            request.request_id = id;
            request.total_lines = lines_count;
            request.file_name = fileName;
            request.status = Enuns.LegacyFileStatus.RECEIVED;
            request.Lines = new List<Models.Legacy.LegacyRequestLine>();

            await DataContext.Requests.AddAsync(request);
            await DataContext.SaveChangesAsync();

            await KafkaManager.SendProcessFileAsync(BkpPatch + id.ToString(), request.request_id);

            return request;
        }
    }
}
