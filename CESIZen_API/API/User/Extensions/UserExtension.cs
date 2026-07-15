// Extension de gestion des exceptions spécifiques au module User.
// Middleware alternatif au handler global de Program.cs, conservé pour référence.
// Note : en production, c'est le handler global de Program.cs qui est actif.

namespace CESIZen_API.API.User.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Middleware de gestion des exceptions métier du module User.
        /// Intercepte UnauthorizedAccessException (401), KeyNotFoundException (404)
        /// et InvalidOperationException (400) avant qu'elles n'atteignent le client.
        /// </summary>
        public static void UseUserExceptionHandler(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (UnauthorizedAccessException ex)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { error = ex.Message });
                }
                catch (KeyNotFoundException ex)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsJsonAsync(new { error = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new { error = ex.Message });
                }
            });
        }
    }
}
