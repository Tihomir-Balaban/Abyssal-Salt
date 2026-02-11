namespace AbySalto.Mid.Application.Contracts;

public interface IPasswordHasher
{
    void CreateHash(string password, out byte[] hash, out byte[] salt);
    bool Verify(string password, byte[] hash, byte[] salt);
}