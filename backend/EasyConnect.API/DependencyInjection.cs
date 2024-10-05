using System;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using EasyConnect.API.Data;
using EasyConnect.API.ExternalServicec.Cloudinary;
using EasyConnect.API.ExternalServices.Cloudinary;
using EasyConnect.API.Helpers;
using EasyConnect.API.Models;
using EasyConnect.API.Security.CurrentUserProvider;
using EasyConnect.API.Security.TokenGenerator;
using EasyConnect.API.Services.AuthService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Shopizy.Infrastructure.Security.TokenGenerator;

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

        services.AddCors(options =>
        {
            options.AddPolicy(
                name: "_myAllowSpecificOrigins",
                builder =>
                {
                    builder
                        .SetIsOriginAllowed((host) => true)
                        .WithOrigins(Origins())
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            );
        });

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
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        _ = services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(configuration.GetSection("JwtSettings:Secret").Value)
                    ),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration.GetSection("JwtSettings:Issuer").Value,
                    ValidAudience = configuration.GetSection("JwtSettings:Audience").Value,
                };

                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync("You are not Authorized");
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync(
                            "You are not authorized to access this resource"
                        );
                    },
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

        services.AddEndpointsApiExplorer().AddSwaggerGen();

        services.AddCors();

        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

        services.AddAutoMapper(typeof(MemberRepository).Assembly);

        services
            .AddScoped<IMemberRepository, MemberRepository>()
            .AddScoped<IAdminRepository, AdminRepository>();

        services.AddScoped<LogUserActivity>();

        services.AddScoped<Seed>();

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
        services.AddSignalR();

        services.AddScoped<IAuthService, AuthService>();
        
    }

    private static string[] Origins()
    {
        return ["http://localhost:4200"];
    }
}
