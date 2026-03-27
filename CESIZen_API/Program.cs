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

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();