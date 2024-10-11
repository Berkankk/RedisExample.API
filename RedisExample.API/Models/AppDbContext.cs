using Microsoft.EntityFrameworkCore;

namespace RedisExample.API.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) //Ctor geçmemizin amacı contextin configuration işlemini program cs de yazmak için
        {

        }

        public DbSet<Product> Products { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new Product() { ProductID = 1, Name = "Kalem1" },
                new Product() { ProductID = 2, Name = "Kalem2" },
                new Product() { ProductID = 3, Name = "Kalem3" });
            base.OnModelCreating(modelBuilder);
        }





    }
}
