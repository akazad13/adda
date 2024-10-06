using System.Collections.Generic;
using System.Threading.Tasks;
using Adda.API.Dtos;
using Adda.API.Helpers;
using Adda.API.Models;
using ErrorOr;

namespace Adda.API.Services.UserService;

public interface IUserService
{
    Task<ErrorOr<User>> RegistrationAsync(RegistrationRequest request);
    Task<PageList<User>> GetAsync(UserParams filterOptions);
    Task<ErrorOr<User>> GetAsync(int id);
    Task<ErrorOr<Success>> UpdateAsync(int id, UserUpdateRequest request);
    Task<ErrorOr<Success>> BookmakAsync(int id, int recipientId);
    Task<IEnumerable<object>> GetUsersWithRolesAsync();
    Task<ErrorOr<IList<string>>> EditRolesAsync(string userName, EditRoleRequest request);
}
