using luizalabs.desafio_tecnico.Data;
using Microsoft.EntityFrameworkCore;

namespace luizalabs.desafio_tecnico.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            dbContext.Database.Migrate();
        }
    }
}
