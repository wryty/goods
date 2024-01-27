using GoodsApi.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography.Xml;

namespace GoodsApi.Data;
public class DataContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        Database.Migrate();
    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {   
        base.OnModelCreating(modelBuilder);
    }
}