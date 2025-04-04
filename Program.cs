using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApi.Services.Interfaces;
using WebApi.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUserServices, UserServices>();

builder.Services.AddDbContext<FirstRunDbContext>(builder => {
    builder.UseNpgsql("Host=localhost;Database=api;Username=postgres;Password=admin;");
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
// builder.Services.AddScalarApi();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";//Redirect path for login  
        options.LogoutPath = "/api/auth/logout"; // Redirect path for Logout
        options.AccessDeniedPath = "/api/auth/forbidden"; // Redirect when access is denied
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20); //Cookie expiration in 20 min
        options.SlidingExpiration = true;
    });
builder.Services.AddHttpContextAccessor();

//Add Authorization Services and Define Policy
builder.Services.AddAuthorization(options=> 
{
    options.AddPolicy("AgeMustBe18+", policy=>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
        {
            var ageClaim = context.User.FindFirst("Age");
            if(ageClaim == null){return false;}
            return int.TryParse(ageClaim.Value, out var age) && age >=18;
        });
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
}
// if (app.Environment.IsDevelopment())
// {
//     app.UseOpenApi();
//     app.MapOpenApi();
// }

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
