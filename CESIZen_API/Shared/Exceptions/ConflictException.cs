namespace CESIZen_API.Shared.Exceptions;

/// <summary>
/// Levée quand une ressource entre en conflit avec une donnée existante (ex. : email déjà utilisé).
/// Mappée en HTTP 409 Conflict par le handler global.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
