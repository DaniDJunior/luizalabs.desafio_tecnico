using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.Legacy;
using luizalabs.desafio_tecnico.Models.Order;
using luizalabs.desafio_tecnico.Models.User;
using Microsoft.EntityFrameworkCore;

namespace luizalabs.desafio_tecnico.Data
{
    public class LegacyData : ILegacyData
    {
        private readonly ILogger<LegacyData> Logger;
        private readonly IServiceProvider ServiceProvider;

        public LegacyData(ILogger<LegacyData> logger, IServiceProvider serviceProvider)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
        }

        public async Task<Models.Legacy.LegacyRequest?> GetByIdAsync(Guid id)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                Models.Legacy.LegacyRequest? request = await dataContext.Requests.Include(r => r.lines).Include(r => r.errors).FirstOrDefaultAsync(request => request.request_id == id);
                return request;
            }
        }

        public async Task<List<Models.Legacy.LegacyRequest>> GetListAsync()
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                List<Models.Legacy.LegacyRequest> requests = await dataContext.Requests.Include(r => r.lines).Include(r => r.errors).ToListAsync();
                return requests;
            }
        }

        public List<Models.Legacy.LegacyRequest> FindByFileName(string fileName)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                List<Models.Legacy.LegacyRequest> requests = dataContext.Requests.Include(r => r.lines).Include(r => r.errors).Where(request => request.file_name == fileName).ToList();
                return requests;
            }
        }

        public async Task<Models.Legacy.LegacyRequest> LoadAsync(Models.Legacy.LegacyRequest request)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                request.lines = await dataContext.RequestsLines.Where(line => line.request_id == request.request_id).ToListAsync();
                return request;
            }
        }

        public async Task<Models.Legacy.LegacyRequest> SaveAsync(Models.Legacy.LegacyRequest request)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                if (await dataContext.Requests.FirstOrDefaultAsync(r => r.request_id == request.request_id) == null)
                {
                    await dataContext.Requests.AddAsync(request);
                }
                else
                {
                    dataContext.Requests.Update(request);
                }

                await dataContext.SaveChangesAsync();

                return request;
            }
        }

        public async Task<Models.Legacy.LegacyRequest> AddLineAsync(Models.Legacy.LegacyRequest request, Models.Legacy.LegacyRequestLine requestLine)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                if (requestLine.request_line_id == Guid.Empty) 
                {
                    requestLine.request = request;
                    requestLine.request_id = request.request_id;
                    requestLine.request_line_id = Guid.NewGuid();

                    request.lines.Add(requestLine);

                    dataContext.Entry(requestLine).State = EntityState.Added;
                }
                else
                {
                    dataContext.Entry(requestLine).State = EntityState.Modified;
                }

                await dataContext.SaveChangesAsync();

                return request;
            }
        }

        public async Task<Models.Legacy.LegacyRequest> AddErrorAsync(Models.Legacy.LegacyRequest request, int lineNumber, string message)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                LegacyRequestError requestError = new LegacyRequestError();

                requestError.request_id = request.request_id;
                requestError.request = request;
                requestError.message = message;
                requestError.line_number = lineNumber;
                requestError.level = 2;

                request.errors.Add(requestError);

                dataContext.Entry(requestError).State = EntityState.Added;

                request.status = Enuns.LegacyFileStatus.ERROR;

                dataContext.Entry(request).State = EntityState.Modified;

                await dataContext.SaveChangesAsync();

                return request;
            }
        }

        public async Task<Models.Legacy.LegacyRequest> AddWarningAsync(Models.Legacy.LegacyRequest request, int lineNumber, string message)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                LegacyRequestError requestError = new LegacyRequestError();

                requestError.request_id = request.request_id;
                requestError.request = request;
                requestError.message = message;
                requestError.line_number = lineNumber;
                requestError.level = 1;

                request.errors.Add(requestError);

                dataContext.Entry(requestError).State = EntityState.Added;

                await dataContext.SaveChangesAsync();

                return request;
            }
        }

        public async Task<LegacyRequestLine?> GetLineByLineNumberAsync(Guid requestId, int lineNumber)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                Models.Legacy.LegacyRequestLine? requestLine = await dataContext.RequestsLines.Include(rl => rl.request).FirstOrDefaultAsync(rl => rl.request_id == requestId && rl.line_number == lineNumber);
                return requestLine;
            }
        }

        public async Task<LegacyRequestLine?> GetLineByIdAsync(Guid id)
        {
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                Models.Legacy.LegacyRequestLine? requestLine = await dataContext.RequestsLines.Include(rl => rl.request).FirstOrDefaultAsync(rl => rl.request_line_id == id);
                return requestLine;
            }
        }
    }
}
