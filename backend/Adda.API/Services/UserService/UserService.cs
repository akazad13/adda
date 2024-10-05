using System;
using System.Threading.Tasks;
using Adda.API.Data;
using Adda.API.Dtos;
using Adda.API.Helpers;
using Adda.API.Models;
using Adda.API.Security.CurrentUserProvider;
using AutoMapper;
using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace Adda.API.Services.UserService;

public class UserService(IMapper mapper, UserManager<User> userManager, ICurrentUserProvider currentUser, IMemberRepository memberRepository) : IUserService
{
    private readonly IMapper _mapper = mapper;
    private readonly UserManager<User> _userManager = userManager;
    private readonly ICurrentUserProvider _currentUser = currentUser;
    private readonly IMemberRepository _memberRepository = memberRepository;

    public async Task<ErrorOr<Success>> BookmakAsync(int id, int recipientId)
    {
        Bookmark bookmark = await _memberRepository.GetBookmarkAsync(id, recipientId);

        if (bookmark != null)
        {
            return Error.Conflict("You already bookmark this user.");
        }

        if (await _memberRepository.GetUserAsync(recipientId, false) == null)
        {
            return Error.Validation("Please select a valid user.");
        }

        var newBookmark = new Bookmark { BookmarkerId = id, BookmarkedId = recipientId };

        await _memberRepository.AddAsync(newBookmark);

        if (await _memberRepository.SaveAllAsync())
        {
            return Result.Success;
        }
        return Error.Validation("Unable to perform the operation.");
    }
    public async Task<ErrorOr<PageList<User>>> GetAsync(UserParams filterOptions)
    {
        filterOptions.UserId = _currentUser.UserId;
        PageList<User> users = await _memberRepository.GetUsersAsync(filterOptions);
        return users;
    }
    public async Task<ErrorOr<User>> GetAsync(int id)
    {
        bool isCurrentUser = _currentUser.UserId == id;
        return await _memberRepository.GetUserAsync(id, isCurrentUser);
    }

    public async Task<ErrorOr<User>> RegistrationAsync(UserForRegisterDto request)
    {
        try
        {

            User userToCreate = _mapper.Map<User>(request);

            IdentityResult result = await _userManager.CreateAsync(userToCreate, request.Password);

            if (result.Succeeded)
            {
                return userToCreate;

            }
            return Error.Failure(description: "Couldn't create user!");
        }
        catch (Exception e)
        {
            return Error.Failure(description: e.Message);
        }
    }

    public async Task<ErrorOr<Success>> UpdateAsync(int id, UserForUpdateDto userForUpdateDto)
    {
        User userFromRepo = await _memberRepository.GetUserAsync(id, true);

        _mapper.Map(userForUpdateDto, userFromRepo); // (from, to)

        if (await _memberRepository.SaveAllAsync())
        {
            return Result.Success;
        }
        return Error.Failure(description: "Couldn't update user!");
    }
}
