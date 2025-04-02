using System;
using WebApi.Dto;

namespace WebApi.Services.Interfaces;

public interface IUserServices
{
    Task RegisterUser(RegisterUserDto dto);
    Task Login(LoginUserDto dto);
    Task Logout();
    Task<List<ViewUserDto>> ViewAllUsers();
    Task<ViewUserDto> ViewUserById(int userId);
}
