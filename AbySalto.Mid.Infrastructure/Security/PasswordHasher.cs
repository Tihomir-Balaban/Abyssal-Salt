using System.Security.Cryptography;
using System.Text;
using AbySalto.Mid.Application.Contracts;

namespace AbySalto.Mid.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    public void CreateHash(string password, out byte[] hash, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(16);

        using var hmac = new HMACSHA256(salt);
        hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public bool Verify(string password, byte[] hash, byte[] salt)
    {
        using var hmac = new HMACSHA256(salt);
        var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return CryptographicOperations.FixedTimeEquals(computed, hash);
    }
}