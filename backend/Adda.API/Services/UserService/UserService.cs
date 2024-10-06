using Adda.API.Dtos;
using Adda.API.Helpers;
using Adda.API.Models;
using Adda.API.Repositories.UserRepository;
using Adda.API.Security.CurrentUserProvider;
using Adda.API.Security.Roles;
using AutoMapper;
using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace Adda.API.Services.UserService;

public class UserService(IMapper mapper, UserManager<User> userManager, ICurrentUserProvider currentUser, IUserRepository userRepository) : IUserService
{
    private readonly IMapper _mapper = mapper;
    private readonly UserManager<User> _userManager = userManager;
    private readonly ICurrentUserProvider _currentUser = currentUser;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<ErrorOr<Success>> BookmakAsync(int id, int recipientId)
    {
        Bookmark? bookmark = await _userRepository.GetBookmarkAsync(id, recipientId);

        if (bookmark != null)
        {
            return Error.Conflict(description: "You already bookmark this user.");
        }

        if (await _userRepository.GetAsync(recipientId, false) == null)
        {
            return Error.Validation(description: "Please select a valid user.");
        }

        var newBookmark = new Bookmark { BookmarkerId = id, BookmarkedId = recipientId };

        await _userRepository.AddAsync(newBookmark);

        if (await _userRepository.SaveAllAsync())
        {
            return Result.Success;
        }
        return Error.Validation(description: "Unable to perform the operation.");
    }
    public async Task<PageList<User>> GetAsync(UserParams filterOptions)
    {
        filterOptions.UserId = _currentUser.UserId;
        PageList<User> users = await _userRepository.GetAsync(filterOptions);
        return users;
    }
    public async Task<ErrorOr<User>> GetAsync(int id)
    {
        bool isCurrentUser = _currentUser.UserId == id;
        return await _userRepository.GetAsync(id, isCurrentUser);
    }

    public async Task<ErrorOr<User>> RegistrationAsync(RegistrationRequest request)
    {
        try
        {
            User? user = await _userManager.FindByNameAsync(request.Username);

            if(user != null)
            {
                return Error.Validation(description: "Username already exists.");
            }

            User userToCreate = _mapper.Map<User>(request);

            IdentityResult result = await _userManager.CreateAsync(userToCreate, request.Password);

            if (result.Succeeded)
            {
                _ = await _userManager.AddToRolesAsync(userToCreate, [RoleOption.Member]);
                return userToCreate;

            }
            return Error.Failure(description: "Couldn't create user!");
        }
        catch (Exception e)
        {
            return Error.Failure(description: e.Message);
        }
    }

    public async Task<ErrorOr<Success>> UpdateAsync(int id, UserUpdateRequest request)
    {
        User? userFromRepo = await _userRepository.GetAsync(id, true);

        if(userFromRepo == null)
        {
            return Error.Failure(description: "User not found.");
        }

        _mapper.Map(request, userFromRepo); // (from, to)

        if (await _userRepository.SaveAllAsync())
        {
            return Result.Success;
        }
        return Error.Failure(description: "Couldn't update user!");
    }

    public async Task<IEnumerable<object>> GetUsersWithRolesAsync()
    {
        return await _userRepository.GetUsersWithRolesAsync();
    }

    public async Task<ErrorOr<IList<string>>> EditRolesAsync(string userName, EditRoleRequest request)
    {
        try
        {
            User? user = await _userManager.FindByNameAsync(userName);

            if(user == null)
            {
                return Error.Failure(description: "User not found.");
            }

            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            string[] selectedRoles = request.RoleName;

            selectedRoles ??= [];
            IdentityResult result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
            {
                return Error.Failure(description: "Failed to add to roles");
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
            {
                return Error.Failure(description: "Failed to remove the roles");
            }

            return (await _userManager.GetRolesAsync(user)).ToList();
        }
        catch (Exception e)
        {
            return Error.Failure(description: e.Message);
        }
    }
}
