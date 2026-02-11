using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;

namespace AbySalto.Mid.Application.Services;

public sealed class AuthService(
    IUserRepository users,
    IPasswordHasher hasher,
    ITokenService tokens)
{
    public async Task<(bool ok, string? error)> RegisterAsync(string email, string password, CancellationToken cancellationToken)
    {
        email = email.Trim().ToLowerInvariant();

        var existing = await users.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
            return (false, "Email already exists.");

        hasher.CreateHash(password, out var hash, out var salt);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            CreatedAtUtc = DateTime.UtcNow
        };

        await users.AddAsync(user, cancellationToken);
        await users.SaveChangesAsync(cancellationToken);

        return (true, null);
    }

    public async Task<(bool ok, string? token, string? error)> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        email = email.Trim().ToLowerInvariant();

        var user = await users.GetByEmailAsync(email, cancellationToken);
        if (user is null)
            return (false, null, "Invalid credentials.");

        var valid = hasher.Verify(password, user.PasswordHash, user.PasswordSalt);
        if (!valid)
            return (false, null, "Invalid credentials.");

        var token = tokens.CreateAccessToken(user.Id, user.Email);
        return (true, token, null);
    }
}