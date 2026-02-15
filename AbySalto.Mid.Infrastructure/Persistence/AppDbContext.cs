using AbySalto.Mid.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Mid.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Basket> Baskets => Set<Basket>();
    public DbSet<BasketItem> BasketItems => Set<BasketItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ExternalId);

            entity.HasIndex(x => x.ExternalId)
                .IsUnique()
                .HasFilter("[ExternalId] IS NOT NULL");
            
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

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserId);

            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();

            entity.Property(x => x.Total)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.HasMany(x => x.Items)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.UserId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ProductId)
                .IsRequired();

            entity.Property(x => x.ProductName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.Quantity)
                .IsRequired();

            entity.HasIndex(x => x.OrderId);
        });
        
        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.ToTable("Favorites");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserId)
                .IsRequired();

            entity.Property(x => x.ProductId)
                .IsRequired();

            entity.HasIndex(x => new { x.UserId, x.ProductId })
                .IsUnique();

            entity.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}