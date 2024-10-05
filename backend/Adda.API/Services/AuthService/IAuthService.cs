using System.Threading.Tasks;
using Adda.API.Dtos;
using ErrorOr;

namespace Adda.API.Services.AuthService;

public interface IAuthService
{
    Task<ErrorOr<AuthResponse>> LoginAsync(UserForLoginDto request);
    Task<ErrorOr<UserForDetailedDto>> RegistrationAsync(UserForRegisterDto request);
}
