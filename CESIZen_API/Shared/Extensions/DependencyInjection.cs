// Extension de configuration de l'injection de dépendances.
// Centralise l'enregistrement de tous les services, repositories, JWT, Swagger, EF Core et CORS
// dans une seule méthode d'extension InjectDependencies() appelée dans Program.cs.
// Chaque méthode privée correspond à une catégorie de configuration.

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;
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
        /// <summary>Issuer/Audience du JWT — partagés entre la validation (ici) et la génération (UserService).</summary>
        public const string JwtIssuer   = "CESIZen_API";
        public const string JwtAudience = "CESIZen_Clients";

        /// <summary>
        /// Point d'entrée unique : enregistre tous les services, repositories et middlewares.
        /// Appelé dans Program.cs avant builder.Build().
        /// </summary>
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
            builder.AddRateLimiting();
        }

        /// <summary>
        /// Politique de limitation de débit "LoginPolicy" : 5 tentatives par minute et par IP sur
        /// les endpoints d'authentification sensibles (login, forgot-password), pour limiter les
        /// attaques par force brute sur les mots de passe.
        /// </summary>
        public static void AddRateLimiting(this WebApplicationBuilder builder)
        {
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("LoginPolicy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window      = TimeSpan.FromMinutes(1),
                            QueueLimit  = 0
                        }));
            });
        }

        /// <summary>
        /// Enregistre les services métier et le service email.
        /// Les factories du pattern Factory (exercices de respiration) sont aussi enregistrées ici.
        /// </summary>
        public static void AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IExerciceService, ExerciceService>();
            builder.Services.AddScoped<IConfigRespirationService, ConfigRespirationService>();
            builder.Services.AddScoped<IInformationService, InformationService>();
            builder.Services.AddScoped<IEmailService, SmtpEmailService>();

            // Factory pattern — chaque implémentation est enregistrée séparément,
            // le resolver les regroupe dans un dictionnaire (type → factory)
            builder.Services.AddScoped<IExerciceRespirationFactory, Technique748Factory>();
            builder.Services.AddScoped<IExerciceRespirationFactory, Technique55Factory>();
            builder.Services.AddScoped<IExerciceRespirationFactory, Technique46Factory>();
            builder.Services.AddScoped<ExerciceRespirationFactoryResolver>();
        }

        /// <summary>
        /// Enregistre le repository générique et les repositories spécifiques.
        /// Le repository générique IBaseRepository&lt;T&gt; est enregistré en open generic.
        /// </summary>
        public static void AddRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IExerciceRepository, ExerciceRepository>();
            builder.Services.AddScoped<IConfigRespirationRepository, ConfigRespirationRepository>();
            builder.Services.AddScoped<IInformationRepository, InformationRepository>();
            builder.Services.AddScoped(typeof(IRoleRepository), typeof(RoleRepository));
        }

        /// <summary>
        /// Configure l'authentification JWT Bearer.
        /// Le secret JWT est lu depuis les variables d'environnement (priorité) ou la configuration.
        /// La validation de l'issuer et de l'audience est désactivée (API interne).
        /// </summary>
        public static void AddJWT(this WebApplicationBuilder builder)
        {
            var jwtSecret = builder.Configuration["JWT_SECRET"]
                ?? Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new InvalidOperationException("JWT secret 'JWT_SECRET' not found.");

            var key = Encoding.ASCII.GetBytes(jwtSecret);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = builder.Environment.IsProduction();
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidIssuer              = JwtIssuer,
                    ValidateAudience         = true,
                    ValidAudience            = JwtAudience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(key)
                };
            });

            builder.Services.AddAuthorization();
        }

        /// <summary>
        /// Configure Swagger/OpenAPI avec le support de l'authentification JWT Bearer
        /// pour pouvoir tester les endpoints protégés directement depuis l'interface Swagger UI.
        /// </summary>
        public static void AddSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // Définition du schéma de sécurité Bearer pour Swagger UI
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name   = "Authorization",
                    In     = ParameterLocation.Header,
                    Type   = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                // Applique la sécurité Bearer à tous les endpoints
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        /// <summary>
        /// Configure Entity Framework Core avec le driver Npgsql (PostgreSQL).
        /// La chaîne de connexion est lue depuis la configuration (appsettings ou variables d'env).
        /// </summary>
        public static void AddEFCoreConfiguration(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseNpgsql(connectionString));
        }

        /// <summary>
        /// Configure la politique CORS "AllowFrontend".
        /// Autorise tous les origins localhost (front-end React et mobile en développement)
        /// ainsi que les origines listées dans la variable d'environnement ALLOWED_ORIGINS
        /// (liste séparée par des virgules, ex. l'URL du front-end déployé en production).
        /// </summary>
        public static void ConfigureCors(this WebApplicationBuilder builder)
        {
            var allowedOrigins = (Environment.GetEnvironmentVariable("ALLOWED_ORIGINS") ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.SetIsOriginAllowed(origin =>
                                Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
                                (uri.Host == "localhost" || allowedOrigins.Contains(origin))
                            )
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });
        }
    }
}
