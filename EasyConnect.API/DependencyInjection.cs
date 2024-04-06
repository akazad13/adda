using System;
using System.Text;
using CloudinaryDotNet;
using EasyConnect.API.Data;
using EasyConnect.API.ExternalServicec.Cloudinary;
using EasyConnect.API.ExternalServices.Cloudinary;
using EasyConnect.API.Helpers;
using EasyConnect.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace EasyConnect.API;

public static class DependencyInjection
{
    public static void AddAppInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (environment.IsDevelopment())
        {
            services.AddDbContext<DataContext>(x => x.UseSqlite(connectionString));
        }
        else
        {
            services.AddDbContext<DataContext>(
                x => x.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );
        }

        var builder = services.AddIdentityCore<User>(opt =>
        {
            opt.Password.RequireDigit = false;
            opt.Password.RequiredLength = 4;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireUppercase = false;
        });

        builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
        builder
            .AddEntityFrameworkStores<DataContext>()
            .AddRoleValidator<RoleValidator<Role>>()
            .AddRoleManager<RoleManager<Role>>()
            .AddSignInManager<SignInManager<User>>();

        // add authentication services for Jwt bearer
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(configuration.GetSection("AppSettings:Token").Value)
                    ),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        services
            .AddAuthorizationBuilder()
            .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
            .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));

        services
            .AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft
                    .Json
                    .ReferenceLoopHandling
                    .Ignore;
            });
        builder.Services
            .AddCors()
            .Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"))
            .AddAutoMapper(typeof(DatingRepository).Assembly)
            .AddScoped<IDatingRepository, DatingRepository>()
            .AddScoped<IAdminRepository, AdminRepository>()
            .AddScoped<LogUserActivity>()
            .AddScoped<Seed>();

        services.AddTransient<ICloudinary, Cloudinary>(sp =>
        {
            var acc = new Account(
                configuration.GetValue<string>("CloudinarySettings:CloudName"),
                configuration.GetValue<string>("CloudinarySettings:ApiKey"),
                configuration.GetValue<string>("CloudinarySettings:ApiSecret")
            );
            var cloudinary = new Cloudinary(acc);
            cloudinary.Api.Secure = configuration.GetValue<bool>("CloudinarySettings:Secure");
            return cloudinary;
        });
        services.AddScoped<ICloudinaryService, CloudinaryService>();
    }
}
