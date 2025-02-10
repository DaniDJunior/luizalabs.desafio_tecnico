using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Legacy;
using luizalabs.desafio_tecnico.Models.Order;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luizalabs.desafio_tecnico.test.LocalMock.Data
{
    internal class LegacyData : ILegacyData
    {
        private List<LegacyRequest> Requests { get; set; }

        public LegacyData()
        {
            Requests = new List<LegacyRequest>();
        }

        public async Task<LegacyRequest> AddLineAsync(LegacyRequest request, LegacyRequestLine requestLine)
        {
            if (request.lines.FirstOrDefault(rl => rl.line_number == requestLine.line_number) == null)
            {
                request.lines.Add(requestLine);
            }
            return request;
        }

        public List<LegacyRequest> FindByFileName(string fileName)
        {
            return Requests.Where(r => r.file_name == fileName).ToList();
        }

        public async Task<LegacyRequest?> GetByIdAsync(Guid id)
        {
            return Requests.FirstOrDefault(r => r.request_id == id);
        }

        public async Task<LegacyRequestLine?> GetLineByIdAsync(Guid id)
        {
            LegacyRequest? request = Requests.FirstOrDefault(r => r.lines.FirstOrDefault(rl => rl.request_line_id == id) != null);

            return request != null ? request.lines.First(rl => rl.request_line_id == id) : null;
        }

        public async Task<List<LegacyRequest>> GetListAsync()
        {
            return Requests;
        }

        public async Task<LegacyRequest> LoadAsync(LegacyRequest request)
        {
            return request;
        }

        public async Task<LegacyRequest> SaveAsync(LegacyRequest request)
        {
            if (!Requests.Contains(request))
            {
                if(request.request_id == Guid.Empty)
                {
                    request.request_id = Guid.NewGuid();
                }
                
                Requests.Add(request);
            }
            return request;
        }

        public async Task<LegacyRequestLine?> GetLineByLineNumberAsync(Guid requestId, int lineNumber)
        {
            LegacyRequest? request = Requests.FirstOrDefault(r => r.request_id == requestId);

            return request.lines.FirstOrDefault(rl => rl.line_number == lineNumber);
        }

        public Task<LegacyRequest> AddErrorAsync(LegacyRequest request, int lineNumber, string message)
        {
            throw new NotImplementedException();
        }

        public Task<LegacyRequest> AddWarningAsync(LegacyRequest request, int lineNumber, string message)
        {
            throw new NotImplementedException();
        }
    }
}
