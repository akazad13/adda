using System.Threading.Tasks;
using Adda.API.Dtos;
using Adda.API.Models;
using ErrorOr;

namespace Adda.API.Services.UserService;

public interface IUserService
{
    Task<ErrorOr<User>> RegistrationAsync(UserForRegisterDto request);
}
