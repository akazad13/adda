using System.Threading.Tasks;
using Adda.API.Dtos;
using Adda.API.Helpers;
using Adda.API.Models;
using ErrorOr;

namespace Adda.API.Services.UserService;

public interface IUserService
{
    Task<ErrorOr<User>> RegistrationAsync(UserForRegisterDto request);
    Task<ErrorOr<PageList<User>>> GetAsync(UserParams filterOptions);
    Task<ErrorOr<User>> GetAsync(int id);
    Task<ErrorOr<Success>> UpdateAsync(int id, UserForUpdateDto userForUpdateDto);
    Task<ErrorOr<Success>> BookmakAsync(int id, int recipientId);
}
