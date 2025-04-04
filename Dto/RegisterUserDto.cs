using System;

namespace WebApi.Dto;

public class RegisterUserDto
{
    public string UserName {get; set;}
    public string Email { get; set;}
    public string? Address { get; set;}
    public int Age { get; set;}
    public string Phone { get; set;}
    public string Password { get; set;}
}
