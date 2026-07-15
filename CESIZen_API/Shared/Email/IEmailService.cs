// Interface du service d'envoi d'emails.
// Abstraction permettant d'injecter différentes implémentations (SMTP, mock pour les tests).

namespace CESIZen_API.Shared.Email;

public interface IEmailService
{
    /// <summary>
    /// Envoie un email de réinitialisation de mot de passe contenant le token généré.
    /// </summary>
    /// <param name="toEmail">Adresse email du destinataire.</param>
    /// <param name="resetToken">Token de réinitialisation à inclure dans l'email.</param>
    Task SendPasswordResetEmailAsync(string toEmail, string resetToken);
}
