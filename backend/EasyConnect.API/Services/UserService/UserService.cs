using System;
using System.Threading.Tasks;
using AutoMapper;
using EasyConnect.API.Dtos;
using EasyConnect.API.Models;
using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace EasyConnect.API.Services.UserService;

public class UserService(IMapper mapper, UserManager<User> userManager) : IUserService
{
    public readonly IMapper _mapper = mapper;
    public readonly UserManager<User> _userManager = userManager;

    public async Task<ErrorOr<User>> RegistrationAsync(UserForRegisterDto request)
    {
        try {

            User userToCreate = _mapper.Map<User>(request);

            IdentityResult result = await _userManager.CreateAsync(userToCreate, request.Password);

            if (result.Succeeded)
            {
                return userToCreate;

            }
            return Error.Failure(description: "Couldn't create user!");
        }
        catch(Exception e) {
             return Error.Failure(description: e.Message);
        }
    }
 }
