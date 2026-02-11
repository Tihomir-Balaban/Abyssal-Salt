using AbySalto.Mid.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Mid.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(2000);
        });
    }
}