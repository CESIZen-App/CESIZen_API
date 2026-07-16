// Point d'entrée de l'API CESIZen (.NET 8).
// Configure le pipeline HTTP : gestion globale des exceptions, CORS, authentification JWT et routage.
// Les dépendances (services, repositories, JWT, Swagger, EF Core) sont injectées
// via l'extension InjectDependencies() définie dans DependencyInjection.cs.
// Les variables d'environnement sont chargées depuis un fichier .env grâce à DotNetEnv.

using CESIZen_API.Shared.Exceptions;
using CESIZen_API.Shared.Extensions;
using DotNetEnv;

// Chargement des variables d'environnement depuis le fichier .env (JWT_SECRET, SMTP_*, connexion BDD)
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Injection de toutes les dépendances (services, repositories, JWT, Swagger, EF Core, CORS)
builder.InjectDependencies();

// Healthcheck : utilisé par Render (et tout outil de supervision) pour déterminer si
// l'instance est saine. Sans route sur "/", le healthcheck par défaut de Render (qui teste "/")
// obtenait un 404 et faisait sortir/rentrer l'instance de rotation de façon intermittente.
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

// Documentation Swagger activée uniquement en développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Gestionnaire global d'exceptions : convertit les exceptions métier en réponses HTTP appropriées
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        // Si le code de statut est toujours 200 après une exception, on force 500
        context.Response.StatusCode = context.Response.StatusCode == 200 ? 500 : context.Response.StatusCode;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (error != null)
        {
            var ex = error.Error;
            // Mapping des types d'exception vers les codes HTTP correspondants
            int statusCode = ex switch
            {
                ConflictException           => 409, // conflit de ressource (ex. email déjà utilisé)
                UnauthorizedAccessException => 403, // authentifié mais non autorisé (401 est géré par [Authorize])
                KeyNotFoundException        => 404,
                InvalidOperationException   => 400,
                _                           => 500
            };
            context.Response.StatusCode = statusCode;
            // Corps JSON uniforme pour toutes les erreurs
            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                inner = ex.InnerException?.Message,
                type  = ex.GetType().Name
            });
        }
    });
});

// Activation du CORS (politique "AllowFrontend" : localhost + origines listées dans ALLOWED_ORIGINS)
app.UseCors("AllowFrontend");
// Limitation de débit (protection contre le brute-force sur les endpoints d'authentification)
app.UseRateLimiter();
// Middlewares d'authentification et d'autorisation JWT
app.UseAuthentication();
app.UseAuthorization();
// Mappage des contrôleurs API
app.MapControllers();

app.Run();
