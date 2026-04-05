using CESIZen_API.Shared.Exceptions;
using CESIZen_API.Shared.Extensions;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.InjectDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = context.Response.StatusCode == 200 ? 500 : context.Response.StatusCode;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (error != null)
        {
            var ex = error.Error;
            int statusCode = ex switch
            {
                ConflictException           => 409, // conflit de ressource (ex. email déjà utilisé)
                UnauthorizedAccessException => 403, // authentifié mais non autorisé (401 est géré par [Authorize])
                KeyNotFoundException        => 404,
                InvalidOperationException   => 400,
                _                           => 500
            };
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
    });
});

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();