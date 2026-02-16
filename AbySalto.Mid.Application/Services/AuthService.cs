using AbySalto.Mid.Application.Common.Exceptions;
using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;

namespace AbySalto.Mid.Application.Services;

public sealed class AuthService(
    IUserRepository users,
    IPasswordHasher hasher,
    ITokenService tokens)
{
    public async Task RegisterAsync(string email, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new AppException("Email is required.", statusCode: 400, errorCode: "email_required");

        if (string.IsNullOrWhiteSpace(password))
            throw new AppException("Password is required.", statusCode: 400, errorCode: "password_required");
        
        email = email.Trim().ToLowerInvariant();

        var existing = await users.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
            throw new AppException("Email already exists.", statusCode: 409, errorCode: "email_exists");

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
    }

    public async Task<string> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new AppException("Email is required.", 400, "email_required");

        if (string.IsNullOrWhiteSpace(password))
            throw new AppException("Password is required.", 400, "password_required");
        
        email = email.Trim().ToLowerInvariant();

        var user = await users.GetByEmailAsync(email, cancellationToken);
        if (user is null)
            throw new AppException("Invalid credentials.", 401, "invalid_credentials");

        var valid = hasher.Verify(password, user.PasswordHash, user.PasswordSalt);
        if (!valid)
            throw new AppException("Invalid credentials.", 401, "invalid_credentials");

        return tokens.CreateAccessToken(user.Id, user.Email);
    }
}