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
            builder.Entity<Models.Legacy.LegacyRequest>(entity =>
            {
                entity.ToTable("requests");
            });

            builder.Entity<Models.Legacy.LegacyRequestLine>(entity =>
            {
                entity.ToTable("requests_lines");
            });

            builder.Entity<Models.User.User>(entity =>
            {
                entity.ToTable("users");
            });

            builder.Entity<Models.Order.Order>(entity =>
            {
                entity.ToTable("orders");
            });

            builder.Entity<Models.Order.OrderProduct>(entity =>
            {
                entity.ToTable("orders_products");
            });

            builder.Entity<Models.Product.Product>(entity =>
            {
                entity.ToTable("products");
            });

            builder.Entity<Models.Legacy.LegacyRequestLine>()
                .HasOne(line => line.request)
                .WithMany(file => file.Lines)
                .HasForeignKey(line => line.request_id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Models.Order.Order>()
                .HasOne(order => order.user)
                .WithMany(user => user.orders)
                .HasForeignKey(order => order.user_id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Models.Order.OrderProduct>()
               .HasKey(order_product => new { order_product.order_id, order_product.product_id });

            builder.Entity<Models.Order.OrderProduct>()
                .HasOne(order_product => order_product.order)
                .WithMany(order => order.products)
                .HasForeignKey(order_product => order_product.order_id)
                .OnDelete(DeleteBehavior.Restrict);

        }

        public DbSet<Models.Legacy.LegacyRequest> Requests { get; set; }
        public DbSet<Models.Legacy.LegacyRequestLine> RequestsLines { get; set; }

        public DbSet<Models.User.User> Users { get; set; }
        public DbSet<Models.Order.Order> Orders { get; set; }
        public DbSet<Models.Order.OrderProduct> OrdersProducts { get; set; }
        public DbSet<Models.Product.Product> Products { get; set; }
    }
}
