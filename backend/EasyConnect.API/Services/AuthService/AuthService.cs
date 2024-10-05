using System.Linq;
using System.Threading.Tasks;
using EasyConnect.API.Dtos;
using EasyConnect.API.Models;
using EasyConnect.API.Security.TokenGenerator;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyConnect.API.Services.AuthService;

public class AuthService(IJwtTokenGenerator jwtTokenGenerator, UserManager<User> userManager,
    SignInManager<User> signInManager) : IAuthService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly UserManager<User> _userManager = userManager;
    public async Task<ErrorOr<AuthResponse>> LoginAsync(UserForLoginDto request)
    {
        var user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(
                u =>
                    u.UserName.Equals(
                        request.Username
                    )
            );

        if (user == null)
        {
            return new ErrorOr<AuthResponse>();
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            false
        );

        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.UserName, roles);
            return new AuthResponse(user.Id, user.KnownAs, user.Gender, user.Photos.FirstOrDefault(p => p.IsMain)?.Url, token);
        }
        else
        {
            return new ErrorOr<AuthResponse>();
        }
    }

    public Task<ErrorOr<UserForDetailedDto>> RegistrationAsync(UserForRegisterDto request)
    {
        throw new System.NotImplementedException();
    }
}
