using Microsoft.EntityFrameworkCore;

namespace CarsOwnerApi;

public class AppDbContext : DbContext
{
    public DbSet<Car> Cars { get; set; }
    public DbSet<Owner> Owners { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=db;Port=5432;Username=postgres;Password=mysecretpassword;Database=cars_owner_db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>(e =>
        {
            e.HasData(
                new Car { Id = 1, Brand = "BMW M3", Year = 2020 },
                new Car { Id = 2, Brand = "Toyota Camry", Year = 2018 },
                new Car { Id = 3, Brand = "Audi A4", Year = 2022 }
            );
        });

        modelBuilder.Entity<Owner>(e =>
        {
            e.HasData(
                new Owner { Id = 1, FullName = "Artem Koval", Age = 26 },
                new Owner { Id = 2, FullName = "Oksana Melnyk", Age = 31 },
                new Owner { Id = 3, FullName = "Denys Bondar", Age = 22 }
            );
        });
    }
}