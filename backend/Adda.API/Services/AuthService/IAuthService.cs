using Adda.API.Dtos;
using ErrorOr;

namespace Adda.API.Services.AuthService;

public interface IAuthService
{
    Task<ErrorOr<AuthResponse>> LoginAsync(AuthRequest request);
}
