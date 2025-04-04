using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Transactions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Entity;
using WebApi.Services.Interfaces;

namespace WebApi.Services;

public class UserServices : IUserServices
{   
    private readonly FirstRunDbContext dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserServices(FirstRunDbContext dbContext,IHttpContextAccessor httpContextAccessor){
        this.dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    //View Users
    public async Task<List<ViewUserDto>> ViewAllUsers(){        
        var users = await dbContext.Users
                .Select(u => new ViewUserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Address = u.Address,
                    Email = u.Email,
                    Phone = u.Phone,
                    CreatedAt = u.CreateAt 
                }).ToListAsync();
        return users;
    }

    //View User by id
    public async Task<ViewUserDto> ViewUserById(int id){
        var user = await dbContext.Users.Where(u => u.Id == id)
                .Select(u => new ViewUserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Address = u.Address,
                    Email = u.Email,
                    Phone = u.Phone,
                    CreatedAt = u.CreateAt 
                })
                .FirstOrDefaultAsync();
        if(user == null){
            throw new Exception("User not Found");
        }
        return user;
    }

    //Register new Users
    public async Task RegisterUser(RegisterUserDto dto){
         using var txn = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //Check if Username or Email Exists
        bool userNameExists = await dbContext.Users.AnyAsync(u => u.UserName ==  dto.UserName);
        bool emailExists = await dbContext.Users.AnyAsync(u => u.Email == dto.Email);

        if(userNameExists || emailExists){
            throw new Exception("Username or Email Already Exists");
        }

        // Generate salt and hash password
        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // 128-bit salt

        // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
        string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: dto.Password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

         var user = new User();
         user.UserName = dto.UserName;
         user.Address = dto.Address;
         user.Email = dto.Email;
         user.Phone = dto.Phone;
         user.Password = hashedPassword;
         user.Salt = Convert.ToBase64String(salt);
         user.CreateAt = DateTime.UtcNow;

         dbContext.Users.Add(user);
         await dbContext.SaveChangesAsync();
         txn.Complete();
    }


    //Code for Login   
    public async Task Login(LoginUserDto dto){
        using var txn = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName || u.Email == dto.Email);

        if(user == null){
            throw new Exception ("Invalid Username or Password.");
        } 

        byte[] salt = Convert.FromBase64String(user.Salt); 
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: dto.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        if(hashedPassword != user.Password){
            throw new Exception("Incorrect Password");
        }

        var httpContext = _httpContextAccessor.HttpContext;
        var claims = new List<Claim>
        {
            new("Id",user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new("Age", user.Age.ToString())
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

        txn.Complete();
    }
    
    //Logout code
    public async Task Logout(){
        await _httpContextAccessor.HttpContext.SignOutAsync();
    }
}
