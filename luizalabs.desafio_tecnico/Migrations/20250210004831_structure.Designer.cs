﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using luizalabs.desafio_tecnico.Data;

#nullable disable

namespace luizalabs.desafio_tecnico.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20250210004831_structure")]
    partial class structure
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("luizalabs.desafio_tecnico.Models.Legacy.LegacyRequest", b =>
                {
                    b.Property<Guid>("request_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("file_name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("status")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("total_lines")
                        .HasColumnType("int");

                    b.HasKey("request_id");

                    b.ToTable("requests", (string)null);
                });

            modelBuilder.Entity("luizalabs.desafio_tecnico.Models.Legacy.LegacyRequestError", b =>
                {
                    b.Property<Guid>("request_error_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("level")
                        .HasColumnType("int");

                    b.Property<int>("line_number")
                        .HasColumnType("int");

                    b.Property<string>("message")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("request_id")
                        .HasColumnType("char(36)");

                    b.HasKey("request_error_id");

                    b.HasIndex("request_id");

                    b.ToTable("requests_errors", (string)null);
                });

            modelBuilder.Entity("luizalabs.desafio_tecnico.Models.Legacy.LegacyRequestLine", b =>
                {
                    b.Property<Guid>("request_line_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("line_number")
                        .HasColumnType("int");

                    b.Property<DateTime>("order_date")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("order_id")
                        .HasColumnType("int");

                    b.Property<int>("product_id")
                        .HasColumnType("int");

                    b.Property<float>("product_value")
                        .HasColumnType("float");

                    b.Property<Guid>("request_id")
                        .HasColumnType("char(36)");

                    b.Property<int>("user_id")
                        .HasColumnType("int");

                    b.Property<string>("user_name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("request_line_id");

                    b.HasIndex("request_id");

                    b.ToTable("requests_lines", (string)null);
                });

            modelBuilder.Entity("luizalabs.desafio_tecnico.Models.Order.Order", b =>
                {
                    b.Property<Guid>("order_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("date")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("legacy_order_id")
                        .HasColumnType("int");

                    b.Property<Guid>("user_id")
                        .HasColumnType("char(36)");

                    b.HasKey("order_id");

                    b.HasIndex("user_id");

                    b.ToTable("orders", (string)null);
                });

            modelBuilder.Entity("luizalabs.desafio_tecnico.Models.Order.OrderProduct", b =>
                {
                    b.Property<Guid>("product_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int?>("legacy_product_id")
                        .HasColumnType("int");

                    b.Property<Guid>("order_id")
                        .HasColumnType("char(36)");

                    b.Property<float>("value")
                        .HasColumnType("float");

                    b.HasKey("product_id");

                    b.HasIndex("order_id");

                    b.ToTable("orders_products", (string)null);
                });

            modelBuilder.Entity("luizalabs.desafio_tecnico.Models.User.User", b =>
                {
                    b.Property<Guid>("user_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int?>("legacy_user_id")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("user_id");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("luizalabs.desafio_tecnico.Models.Legacy.LegacyRequestError", b =>
                {
                    b.HasOne("luizalabs.desafio_tecnico.Models.Legacy.LegacyRequest", "request")
                        .WithMany("errors")
                        .HasForeignKey("request_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("request");
                });

            modelBuilder.Entity("luizalabs.desafio_tecnico.Models.Legacy.LegacyRequestLine", b =>
                {
                    b.HasOne("luizalabs.desafio_tecnico.Models.Legacy.LegacyRequest", "request")
                        .WithMany("lines")
                        .HasForeignKey("request_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("request");
                });

            modelBuilder.Entity("luizalabs.desafio_tecnico.Models.Order.Order", b =>
                {
                    b.HasOne("luizalabs.desafio_tecnico.Models.User.User", "user")
                        .WithMany("orders")
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("luizalabs.desafio_tecnico.Models.Order.OrderProduct", b =>
                {
                    b.HasOne("luizalabs.desafio_tecnico.Models.Order.Order", "order")
                        .WithMany("products")
                        .HasForeignKey("order_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("order");
                });

            modelBuilder.Entity("luizalabs.desafio_tecnico.Models.Legacy.LegacyRequest", b =>
                {
                    b.Navigation("errors");

                    b.Navigation("lines");
                });

            modelBuilder.Entity("luizalabs.desafio_tecnico.Models.Order.Order", b =>
                {
                    b.Navigation("products");
                });

            modelBuilder.Entity("luizalabs.desafio_tecnico.Models.User.User", b =>
                {
                    b.Navigation("orders");
                });
#pragma warning restore 612, 618
        }
    }
}
