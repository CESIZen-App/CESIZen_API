using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CESIZen_API.Shared.Validators;

public class PasswordValidator : ValidationAttribute
{
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

        var hasNumber = new Regex(@"[0-9]{2,}");
        var hasUpperLetters = new Regex(@"[A-Z]{2,}");
        var hasLowerCase = new Regex(@"[a-z]{2,}");
        var hasEnoughChars = new Regex(@".{8,15}");
        var hasSymbol = new Regex(@"[.+*?!:;,^@/$(){}|]{3,}");

        if (hasNumber.IsMatch(input) == false)
        {
            messages.Add("Il manque des chiffres.");
        }
        if (hasUpperLetters.IsMatch(input) == false)
        {
            messages.Add("Il manque des lettres majuscules.");
        }
        if (hasLowerCase.IsMatch(input) == false)
        {
            messages.Add("Il manque des lettres minuscules.");
        }

        if (hasEnoughChars.IsMatch(input) == false)
        {
            messages.Add("On doit avoir entre 8 et 15 caractères.");
        }

        if (hasSymbol.IsMatch(input) == false)
        {
            messages.Add("Il manque des symboles.");
        }

        ErrorMessage = string.Join("\n", messages);

        return string.IsNullOrEmpty(ErrorMessage);
    }
}