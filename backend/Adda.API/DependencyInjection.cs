using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Adda.API.Data;
using Adda.API.ExternalServicec.Cloudinary;
using Adda.API.ExternalServices.Cloudinary;
using Adda.API.Helpers;
using Adda.API.Models;
using Adda.API.Repositories.MessageRepository;
using Adda.API.Repositories.PhotoRepository;
using Adda.API.Repositories.UserRepository;
using Adda.API.Security.CurrentUserProvider;
using Adda.API.Security.TokenGenerator;
using Adda.API.Services.AuthService;
using Adda.API.Services.MessageService;
using Adda.API.Services.PhotoService;
using Adda.API.Services.UserService;
using CloudinaryDotNet;
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

namespace Adda.API;

public static class DependencyInjection
{
    public static void AddAppInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddControllers(options =>
            {
                AuthorizationPolicy policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
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
        services.AddScoped<LogUserActivity>();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.RegisterServices();
        services.AddInfrastructure(configuration, environment);

    }

    private static void RegisterServices(this IServiceCollection services)
    {
        _ = services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IPhotoService, PhotoService>()
            .AddScoped<IMessageService, MessageService>();
    }

    private static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

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

        return services
            .AddHttpContextAccessor()
            .AddExternalServices(configuration)
            .AddAuthentication(configuration)
            .AddAuthorization()
            .AddPersistence(configuration, environment)
            .AddRepositories();
    }

    public static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services
            .AddAuthorizationBuilder()
            .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
            .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));

        return services;
    }

    public static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        _ = services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));
        _ = services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

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
                         Microsoft.Extensions.Primitives.StringValues accessToken = context.Request.Query["access_token"];
                         PathString path = context.HttpContext.Request.Path;

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

        return services;
    }

    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment
    )
    {

        string connectionString = configuration.GetConnectionString("DefaultConnection");
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

        services.AddScoped<Seed>();

        IdentityBuilder builder = services.AddIdentityCore<User>(opt =>
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
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IPhotoRepository, PhotoRepository>()
            .AddScoped<IMessageRepository, MessageRepository>();
    }

    private static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
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

        return services;
    }

    private static string[] Origins() => ["http://localhost:4200"];
}
