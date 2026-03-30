using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using CESIZen_API.API.ConfigRespiration.Repositories;
using CESIZen_API.API.ConfigRespiration.Services;
using CESIZen_API.API.Exercice.Factory;
using CESIZen_API.API.Exercice.Factory.Implementations;
using CESIZen_API.API.Exercice.Repositories;
using CESIZen_API.API.Exercice.Services;
using CESIZen_API.API.Information.Repositories;
using CESIZen_API.API.Information.Services;
using CESIZen_API.Shared.Email;
using CESIZen_API.API.Role.Repositories;
using CESIZen_API.API.Role.Services;
using CESIZen_API.API.User.Repositories;
using CESIZen_API.API.User.Services;
using CESIZen_API.Shared.Data;
using CESIZen_API.Shared.Repositories;

namespace CESIZen_API.Shared.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void InjectDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.AddServices();
            builder.AddRepositories();
            builder.AddJWT();
            builder.AddSwagger();
            builder.AddEFCoreConfiguration();
            builder.ConfigureCors();
        }

        public static void AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IExerciceService, ExerciceService>();
            builder.Services.AddScoped<IConfigRespirationService, ConfigRespirationService>();
            builder.Services.AddScoped<IInformationService, InformationService>();
            builder.Services.AddScoped<IEmailService, SmtpEmailService>();

            // Factory pattern — exercices de respiration
            builder.Services.AddScoped<IExerciceRespirationFactory, Technique748Factory>();
            builder.Services.AddScoped<IExerciceRespirationFactory, Technique55Factory>();
            builder.Services.AddScoped<IExerciceRespirationFactory, Technique46Factory>();
            builder.Services.AddScoped<ExerciceRespirationFactoryResolver>();
        }

        public static void AddRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IExerciceRepository, ExerciceRepository>();
            builder.Services.AddScoped<IConfigRespirationRepository, ConfigRespirationRepository>();
            builder.Services.AddScoped<IInformationRepository, InformationRepository>();

            builder.Services.AddScoped(typeof(IRoleRepository), typeof(RoleRepository));
        }

        public static void AddJWT(this WebApplicationBuilder builder)
        {
            var jwtSecret = builder.Configuration["JWT_SECRET"]
                ?? Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new InvalidOperationException("JWT secret 'JWT_SECRET' not found.");

            var key = Encoding.ASCII.GetBytes(jwtSecret);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Mettre à true en production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            builder.Services.AddAuthorization();
        }

        public static void AddSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        public static void AddEFCoreConfiguration(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseNpgsql(connectionString));
        }

        public static void ConfigureCors(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000")
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });
        }
    }
}