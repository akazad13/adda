using System.Threading.Tasks;
using EasyConnect.API.Dtos;
using ErrorOr;

namespace EasyConnect.API.Services;

public interface IAuthService
{
    Task<ErrorOr<AuthResponse>> LoginAsync(UserForLoginDto request);
    Task<ErrorOr<UserForDetailedDto>> RegistrationAsync(UserForRegisterDto request);
}
