// Validateur de mot de passe personnalisé (DataAnnotations).
// Vérifie les règles de complexité du mot de passe lors de la validation des DTOs :
//   - Au moins 2 chiffres
//   - Au moins 2 lettres majuscules
//   - Au moins 2 lettres minuscules
//   - Entre 8 et 128 caractères (borne haute généreuse pour permettre des phrases de passe longues,
//     recommandées par les standards actuels — NIST SP 800-63B — plutôt qu'un plafond restrictif)
//   - Au moins 3 symboles parmi : . + * ? ! : ; , ^ @ / $ ( ) { } |
// Utilisé sur les propriétés Password des DTOs Register, UpdateUser et ResetPassword.

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CESIZen_API.Shared.Validators;

public class PasswordValidator : ValidationAttribute
{
    /// <summary>
    /// Vérifie les règles de complexité du mot de passe.
    /// Accumule tous les messages d'erreur pour les retourner en une seule réponse.
    /// </summary>
    public override bool IsValid(object? value)
    {
        var input = value?.ToString();
        var messages = new List<string>();
        ErrorMessage = string.Empty;

        if (string.IsNullOrEmpty(input))
        {
            ErrorMessage = "Le mot de passe doit être renseigné";
            return false;
        }

        // Expressions régulières pour chaque règle de complexité
        var hasNumber        = new Regex(@"[0-9]{2,}");
        var hasUpperLetters  = new Regex(@"[A-Z]{2,}");
        var hasLowerCase     = new Regex(@"[a-z]{2,}");
        var hasEnoughChars   = new Regex(@".{8,128}");
        var hasSymbol        = new Regex(@"[.+*?!:;,^@/$(){}|]{3,}");

        if (!hasNumber.IsMatch(input))
            messages.Add("Il manque des chiffres.");

        if (!hasUpperLetters.IsMatch(input))
            messages.Add("Il manque des lettres majuscules.");

        if (!hasLowerCase.IsMatch(input))
            messages.Add("Il manque des lettres minuscules.");

        if (!hasEnoughChars.IsMatch(input))
            messages.Add("On doit avoir entre 8 et 128 caractères.");

        if (!hasSymbol.IsMatch(input))
            messages.Add("Il manque des symboles.");

        // Concaténation de tous les messages d'erreur (retournés comme ErrorMessage unique)
        ErrorMessage = string.Join("\n", messages);
        return string.IsNullOrEmpty(ErrorMessage);
    }
}
