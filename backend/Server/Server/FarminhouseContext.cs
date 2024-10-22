using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server;

public partial class FarminhouseContext : DbContext
{
    public FarminhouseContext()
    {
    }

    public FarminhouseContext(DbContextOptions<FarminhouseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentsType> PaymentsTypes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("DataSource=farminhouse.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("INT")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("DATE")
                .HasColumnName("created_at");
            entity.Property(e => e.IsReserved)
                .HasColumnType("INT")
                .HasColumnName("is_reserved");
            entity.Property(e => e.UserId)
                .HasColumnType("INT")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasMany(d => d.Orders).WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "OrdersProduct",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Order>().WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("ProductId", "OrderId");
                        j.ToTable("orders_products");
                        j.IndexerProperty<int>("ProductId")
                            .HasColumnType("INT")
                            .HasColumnName("product_id");
                        j.IndexerProperty<int>("OrderId")
                            .HasColumnType("INT")
                            .HasColumnName("order_id");
                    });
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("INT")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("DATE")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDone)
                .HasColumnType("INT")
                .HasColumnName("is_done");
            entity.Property(e => e.OrderId)
                .HasColumnType("INT")
                .HasColumnName("order_id");
            entity.Property(e => e.PaymentTypeId)
                .HasColumnType("INT")
                .HasColumnName("payment_type_id");
            entity.Property(e => e.Total)
                .HasColumnType("FLOAT")
                .HasColumnName("total");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentType).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PaymentsType>(entity =>
        {
            entity.ToTable("payments_types");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("INT")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("INT")
                .HasColumnName("id");
            entity.Property(e => e.Average)
                .HasColumnType("FLOAT")
                .HasColumnName("average");
            entity.Property(e => e.Description)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("FLOAT")
                .HasColumnName("price");
            entity.Property(e => e.Stock)
                .HasColumnType("INT")
                .HasColumnName("stock");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("reviews");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("INT")
                .HasColumnName("id");
            entity.Property(e => e.ProductId)
                .HasColumnType("INT")
                .HasColumnName("product_id");
            entity.Property(e => e.Score)
                .HasColumnType("INT")
                .HasColumnName("score");
            entity.Property(e => e.Text)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("text");
            entity.Property(e => e.UserId)
                .HasColumnType("INT")
                .HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("INT")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
