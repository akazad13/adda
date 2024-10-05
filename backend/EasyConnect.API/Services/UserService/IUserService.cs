using System.Threading.Tasks;
using EasyConnect.API.Dtos;
using EasyConnect.API.Models;
using ErrorOr;

namespace EasyConnect.API.Services.UserService;

public interface IUserService
{
    Task<ErrorOr<User>> RegistrationAsync(UserForRegisterDto request);
}
