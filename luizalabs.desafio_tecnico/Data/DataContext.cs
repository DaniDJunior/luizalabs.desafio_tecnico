using Microsoft.EntityFrameworkCore;
using System.Data;

namespace luizalabs.desafio_tecnico.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Models.Legacy.LegacyFile>(entity =>
            {
                entity.ToTable("requests");
            });

            builder.Entity<Models.Legacy.LegacyData>(entity =>
            {
                entity.ToTable("requests_lines");
            });

            builder.Entity<Models.Legacy.LegacyData>()
                .HasOne(line => line.file)
                .WithMany(file => file.Lines)
                .HasForeignKey(line => line.request_line_id);

        }

        public DbSet<Models.Legacy.LegacyFile> Requests { get; set; }
        public DbSet<Models.Legacy.LegacyData> RequestsLines { get; set; }
    }
}
