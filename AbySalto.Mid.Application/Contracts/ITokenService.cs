namespace AbySalto.Mid.Application.Contracts;

public interface ITokenService
{
    string CreateAccessToken(Guid userId, string email);
}