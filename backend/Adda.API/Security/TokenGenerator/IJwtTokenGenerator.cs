namespace Adda.API.Security.TokenGenerator;

public interface IJwtTokenGenerator
{
    string GenerateToken(int userId, string userName, IList<string> roles);
}
