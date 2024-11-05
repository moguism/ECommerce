using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server;

public class FarminhouseContext : DbContext
{
    private const string DATABASE_PATH = "farminhouse.db";

    public DbSet<Order> Orders { get; set; }
    public DbSet<PaymentsType> PaymentsTypes { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<User> Users { get; set; }

    public DbSet<Category> Categories { get; set; }

    //Nuevas tablas
    public DbSet<ShoppingCart> ShoppingCart { get; set; }
    public DbSet<TemporalOrder> TemporalOrder { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        optionsBuilder.UseSqlite($"DataSource={baseDir}{DATABASE_PATH}");
    }
}
