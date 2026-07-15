// Implémentation du service d'email via SMTP (MailKit).
// La configuration SMTP est lue depuis les variables d'environnement :
//   SMTP_HOST, SMTP_PORT, SMTP_USER, SMTP_PASSWORD, SMTP_FROM_NAME.
// Utilise StartTLS pour chiffrer la connexion (port 587 par défaut pour Gmail).

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CESIZen_API.Shared.Email;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Envoie un email HTML de réinitialisation de mot de passe avec le token.
    /// Le token expire dans 1 heure (géré côté UserService).
    /// </summary>
    public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
    {
        // Lecture de la configuration SMTP depuis les variables d'environnement
        var host     = _configuration["SMTP_HOST"] ?? "smtp.gmail.com";
        var port     = int.Parse(_configuration["SMTP_PORT"] ?? "587");
        var user     = _configuration["SMTP_USER"]
            ?? throw new InvalidOperationException("SMTP_USER non configuré.");
        var password = _configuration["SMTP_PASSWORD"]
            ?? throw new InvalidOperationException("SMTP_PASSWORD non configuré.");
        var fromName = _configuration["SMTP_FROM_NAME"] ?? "CESIZen";

        // Construction du message MIME avec corps HTML
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, user));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Réinitialisation de votre mot de passe CESIZen";

        message.Body = new TextPart("html")
        {
            Text = $"""
                <h2>Réinitialisation de mot de passe</h2>
                <p>Vous avez demandé la réinitialisation de votre mot de passe.</p>
                <p>Votre token de réinitialisation : <strong>{resetToken}</strong></p>
                <p>Ce token expire dans 1 heure.</p>
                <p>Si vous n'avez pas fait cette demande, ignorez cet email.</p>
                """
        };

        // Connexion SMTP avec StartTLS, authentification puis envoi
        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(user, password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
