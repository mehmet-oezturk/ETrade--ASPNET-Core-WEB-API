using EConverce.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EConverce.Api.DataAccess
{
    public class DatabaseContext:DbContext

    {
        public DatabaseContext( DbContextOptions options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartProduct> CartProducts { get; set; }
        public DbSet<Payment> Payments { get; set; }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if(optionsBuilder.IsConfigured== false)
        //    { 
        //        optionsBuilder.UseSqlServer("Server=localhost;Database=ETicaretAppDB;Trusted_Connection=true");
                
        //     }
        //}


    }
}
