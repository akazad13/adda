using System.Collections.Generic;

namespace EasyConnect.API.Security.TokenGenerator;

public interface IJwtTokenGenerator
{
    string GenerateToken(int userId, string username, IList<string> roles);
}
