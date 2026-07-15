using CESIZen_API.Shared.Utils;
using Xunit;

namespace CESIZen_API.Tests.User;

/// <summary>
/// Tests unitaires sur le hachage de mot de passe (PBKDF2-SHA512).
///
/// Note : l'implémentation utilise PBKDF2-SHA512 (350 000 itérations + sel aléatoire),
/// Ces tests vérifient que le mot de passe n'est jamais stocké en clair et que la
/// vérification fonctionne correctement.
/// </summary>
public class PasswordHashingTests
{
    private const string PlainPassword = "MonMotDePasse!99";

    // ─── Test 1a ────────────────────────────────────────────────────────────────
    /// <summary>
    /// Le hash stocké ne doit jamais être égal au mot de passe en clair.
    /// On vérifie également que le format "hash:sel" ne contient pas le mot de passe
    /// en clair, ni en UTF-8, ni en Base64.
    /// </summary>
    [Fact]
    public void HashPassword_NeverStoresPlainText()
    {
        var stored = PasswordUtils.HashPassword(PlainPassword, out _);

        // Le hash brut n'est pas le mot de passe
        Assert.NotEqual(PlainPassword, stored);

        // Le hash ne contient pas le mot de passe en clair comme sous-chaîne
        Assert.DoesNotContain(PlainPassword, stored, StringComparison.OrdinalIgnoreCase);

        // Le hash ne contient pas le mot de passe encodé en Base64
        var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(PlainPassword));
        Assert.DoesNotContain(base64, stored, StringComparison.OrdinalIgnoreCase);
    }

    // ─── Test 1b ────────────────────────────────────────────────────────────────
    /// <summary>
    /// HashPassword retourne uniquement le hash (hex, 64 octets → 128 chars).
    /// Le sel est retourné séparément via le paramètre out.
    /// C'est UserService qui assemble le format "hash:sel" avant de stocker.
    /// Ce test vérifie les deux sorties indépendamment.
    /// </summary>
    [Fact]
    public void HashPassword_OutputFormat_Is_HexHash_And_SaltIsBytes()
    {
        var hash = PasswordUtils.HashPassword(PlainPassword, out var salt);

        // Hash : 64 octets → 128 caractères hex, uniquement [0-9A-F]
        Assert.Equal(128, hash.Length);
        Assert.Matches("^[0-9A-F]+$", hash);

        // Sel : 64 octets
        Assert.Equal(64, salt.Length);
        Assert.Matches("^[0-9A-F]+$", Convert.ToHexString(salt));

        // Le format de stockage utilisé par UserService doit contenir exactement un ":"
        var stored = $"{hash}:{Convert.ToHexString(salt)}";
        Assert.Equal(2, stored.Split(':').Length);
    }

    // ─── Test 1c ────────────────────────────────────────────────────────────────
    /// <summary>
    /// Deux appels avec le même mot de passe produisent des hashs différents
    /// grâce au sel aléatoire (protection contre les attaques par table arc-en-ciel).
    /// </summary>
    [Fact]
    public void HashPassword_SamePassword_ProducesDifferentHashes()
    {
        var hash1 = PasswordUtils.HashPassword(PlainPassword, out _);
        var hash2 = PasswordUtils.HashPassword(PlainPassword, out _);

        Assert.NotEqual(hash1, hash2);
    }

    // ─── Test 1d ────────────────────────────────────────────────────────────────
    /// <summary>
    /// VerifyPassword doit retourner true pour le bon mot de passe.
    /// </summary>
    [Fact]
    public void VerifyPassword_ReturnsTrue_ForCorrectPassword()
    {
        var stored = PasswordUtils.HashPassword(PlainPassword, out var salt);
        var hash = stored.Split(':')[0];

        Assert.True(PasswordUtils.VerifyPassword(PlainPassword, hash, salt));
    }

    // ─── Test 1e ────────────────────────────────────────────────────────────────
    /// <summary>
    /// VerifyPassword doit retourner false pour un mauvais mot de passe.
    /// </summary>
    [Fact]
    public void VerifyPassword_ReturnsFalse_ForWrongPassword()
    {
        var stored = PasswordUtils.HashPassword(PlainPassword, out var salt);
        var hash = stored.Split(':')[0];

        Assert.False(PasswordUtils.VerifyPassword("MauvaisMotDePasse!", hash, salt));
    }
}
