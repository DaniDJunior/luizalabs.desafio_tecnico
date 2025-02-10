using luizalabs.desafio_tecnico.Models.Order;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data;

namespace luizalabs.desafio_tecnico.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Models.Legacy.LegacyRequest>(entity =>
            {
                entity.ToTable("requests");
            });

            builder.Entity<Models.Legacy.LegacyRequestLine>(entity =>
            {
                entity.ToTable("requests_lines");
            });

            builder.Entity<Models.Legacy.LegacyRequestError>(entity =>
            {
                entity.ToTable("requests_errors");
            });

            builder.Entity<Models.User.User>(entity =>
            {
                entity.ToTable("users");
            });

            builder.Entity<Models.Order.Order>(entity =>
            {
                entity.ToTable("orders");
            });

            builder.Entity<OrderProduct>(entity =>
            {
                entity.ToTable("orders_products");
            });

            builder.Entity<Models.Legacy.LegacyRequestLine>()
                .HasOne(line => line.request)
                .WithMany(request => request.lines)
                .HasForeignKey(line => line.request_id);

            builder.Entity<Models.Legacy.LegacyRequestError>()
                .HasOne(error => error.request)
                .WithMany(request => request.errors)
                .HasForeignKey(line => line.request_id);

            builder.Entity<Models.Order.Order>()
                .HasOne(order => order.user)
                .WithMany(user => user.orders)
                .HasForeignKey(order => order.user_id);

            builder.Entity<OrderProduct>()
                .HasOne(product => product.order)
                .WithMany(order => order.products)
                .HasForeignKey(product => product.order_id);

        }

        public DbSet<Models.Legacy.LegacyRequest> Requests { get; set; }
        public DbSet<Models.Legacy.LegacyRequestLine> RequestsLines { get; set; }
        public DbSet<Models.Legacy.LegacyRequestError> RequestsErrors { get; set; }

        public DbSet<Models.User.User> Users { get; set; }
        public DbSet<Models.Order.Order> Orders { get; set; }
        public DbSet<OrderProduct> OrdersProducts { get; set; }
    }
}
