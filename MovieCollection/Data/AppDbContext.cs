using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieCollection.Models;

namespace MovieCollection.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Обязательно для работы Identity

            // Игнорирование навигационного свойства Movies в Category
            modelBuilder.Entity<Category>().Ignore(c => c.Movies);

            // Настройка точности для поля Price
            modelBuilder.Entity<Movie>()
                .Property(m => m.Price)
                .HasPrecision(18, 2); // decimal(18,2) в SQL Server

            // Дополнительные настройки (пример для CartItem)
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany()
                .HasForeignKey(ci => ci.UserId);
        }
    }
}