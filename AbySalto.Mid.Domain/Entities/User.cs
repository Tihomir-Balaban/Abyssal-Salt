namespace AbySalto.Mid.Domain.Entities;

public sealed class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
