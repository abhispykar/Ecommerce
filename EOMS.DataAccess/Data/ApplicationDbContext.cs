using Microsoft.EntityFrameworkCore;
using EOMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace EOMS.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Category>().HasData(new Category { Id = 1, Name = "Sports Bike", DisplayOrder = 1 }, new Category { Id = 2, Name = "Cruiser Bikes", DisplayOrder = 2 }, new Category { Id = 3, Name = "Off Roading Bikes", DisplayOrder = 3 });

            modelBuilder.Entity<Product>().HasData(
              new Product
              {
                  ProductId = 1,
                  Name = "Yamaha R15 V4",
                  Price = 183465.00m,
                  Stock = 1000,
                  CategoryId = 1,
                  ImageUrl = ""
              },
              new Product
              {
                  ProductId = 2,
                  Name = "TVS Apache RR 310",
                  Price = 275000.00m,
                  Stock = 1000,
                  CategoryId = 2,
                  ImageUrl = ""
              },
              new Product
              {
                  ProductId = 3,
                  Name = "Bajaj Pulsar RS 200",
                  Price = 172698.00m,
                  Stock = 1000,
                  CategoryId = 3,
                  ImageUrl = ""
              }
          );

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.ShippingAddressJson).HasColumnName("ShippingAddress").HasMaxLength(1024);
            });
        }
    }

}
