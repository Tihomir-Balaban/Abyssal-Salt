using AbySalto.Mid.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Mid.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Basket> Baskets => Set<Basket>();
    public DbSet<BasketItem> BasketItems => Set<BasketItem>();

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
        
        modelBuilder.Entity<Basket>(entity =>
        {
            entity.ToTable("Baskets");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserId);

            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();
            entity.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(x => x.BasketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BasketItem>(entity =>
        {
            entity.ToTable("BasketItems");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ProductId)
                .IsRequired();

            entity.Property(x => x.Quantity)
                .IsRequired();

            entity.HasIndex(x => new { x.BasketId, x.ProductId })
                .IsUnique();
        });
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Email).IsUnique();

            entity.Property(x => x.Email)
                .HasMaxLength(320)
                .IsRequired();

            entity.Property(x => x.PasswordHash)
                .IsRequired();

            entity.Property(x => x.PasswordSalt)
                .IsRequired();

            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();
        });

    }
}