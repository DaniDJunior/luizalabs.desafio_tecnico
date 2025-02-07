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

        public async Task<List<Models.Legacy.LegacyFile>> GetListAsync()
        {
            return await DataContext.Requests.ToListAsync();
        }

        public async Task<Models.Legacy.LegacyFile> StartRequestAsync(string fileName, Guid id)
        {
            int lines_count = (await System.IO.File.ReadAllLinesAsync(BkpPatch + id.ToString())).Length;

            Models.Legacy.LegacyFile request = new Models.Legacy.LegacyFile();
            request.request_id = id;
            request.total_lines = lines_count;
            request.file_name = fileName;
            request.status = Enuns.LegacyFileStatus.RECEIVED;
            request.Lines = new List<Models.Legacy.LegacyData>();

            await DataContext.Requests.AddAsync(request);
            await DataContext.SaveChangesAsync();

            await KafkaManager.SendProcessFileAsync(BkpPatch + id.ToString(), request.request_id);

            return request;
        }
    }
}
