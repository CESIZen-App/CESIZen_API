using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CESIZen_API.API.User.DTOs;
using CESIZen_API.API.User.Models;
using CESIZen_API.API.User.Repositories;
using CESIZen_API.API.User.Services;
using CESIZen_API.Shared.Data;
using CESIZen_API.Shared.Email;
using CESIZen_API.Shared.Utils;
using CESIZen_API.Shared.Exceptions; // Ajout du namespace pour les exceptions
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace CESIZen_API.Tests.User;

/// <summary>
/// Helpers partagés entre les tests.
/// </summary>
file static class Helpers
{
    public const string JwtSecret = "CleSecreteTresLongueEtAleatoire_123!ABCDEFGH";

    public static (UserService service, Mock<IUserRepository> repoMock, MyDbContext context) Build(
        string dbName = "")
    {
        Environment.SetEnvironmentVariable("JWT_SECRET", JwtSecret);

        var repoMock = new Mock<IUserRepository>();
        var emailMock = new Mock<IEmailService>();
        emailMock
            .Setup(e => e.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWT_SECRET"] = JwtSecret
            })
            .Build();

        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(string.IsNullOrEmpty(dbName) ? Guid.NewGuid().ToString() : dbName)
            .Options;
        var context = new MyDbContext(options);

        var service = new UserService(repoMock.Object, config, context, emailMock.Object);
        return (service, repoMock, context);
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// TEST 2 — Inscription avec un email déjà utilisé
// ═══════════════════════════════════════════════════════════════════════════════

public class RegisterTests
{
    [Fact]
    public async Task Register_Throws_WhenEmailAlreadyExists()
    {
        var (service, repoMock, _) = Helpers.Build();

        repoMock
            .Setup(r => r.GetByEmailAsync("existant@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserModel { Id = 1, Email = "existant@example.com", Password = "hash:sel", RoleId = 2 });

        var dto = new RegisterDTO { Nom = "Test", Email = "existant@example.com", Password = "Password!99" };

        // CORRECTION ICI : Utilisation de ConflictException au lieu de InvalidOperationException
        var ex = await Assert.ThrowsAsync<ConflictException>(() => service.RegisterAsync(dto));
        Assert.Contains("existe déjà", ex.Message);
    }

    [Fact]
    public async Task Register_Succeeds_WhenEmailIsNew()
    {
        var (service, repoMock, _) = Helpers.Build();

        repoMock
            .Setup(r => r.GetByEmailAsync("nouveau@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);

        repoMock
            .Setup(r => r.AddAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()))
            .Callback<UserModel, CancellationToken>((u, _) => u.Id = 42)
            .ReturnsAsync((UserModel u, CancellationToken _) => u);

        var dto = new RegisterDTO { Nom = "Nouveau", Email = "nouveau@example.com", Password = "Password!99" };

        var result = await service.RegisterAsync(dto);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
    }
}

// ... Le reste du fichier (JwtTokenTests et PasswordResetTokenTests) reste identique 
// sauf si tu as aussi personnalisé les exceptions pour le Reset de mot de passe.
// ═══════════════════════════════════════════════════════════════════════════════
// TEST 3 — Structure et signature du JWT
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Vérifie la structure et la validité cryptographique du token JWT généré.
///
/// Stratégie : on déclenche RegisterAsync (chemin le plus simple qui appelle
/// GenerateToken, qui est private), puis on parse le token retourné avec
/// JwtSecurityTokenHandler pour inspecter son contenu et sa signature.
/// </summary>
public class JwtTokenTests
{
    private static async Task<string> GetToken()
    {
        var (service, repoMock, _) = Helpers.Build();

        repoMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);

        repoMock
            .Setup(r => r.AddAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()))
            .Callback<UserModel, CancellationToken>((u, _) => u.Id = 7)
            .ReturnsAsync((UserModel u, CancellationToken _) => u);

        var dto = new RegisterDTO { Nom = "JwtUser", Email = "jwt@example.com", Password = "Password!99" };
        var result = await service.RegisterAsync(dto);
        return result.Token;
    }

    [Fact]
    public async Task Jwt_Algorithm_Is_HS256()
    {
        var token = await GetToken();
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(SecurityAlgorithms.HmacSha256, jwt.Header.Alg, ignoreCase: true);
    }

    [Fact]
    public async Task Jwt_Contains_Required_Claims()
    {
        var token = await GetToken();
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        // NameIdentifier (sub), Email, Role doivent être présents
        Assert.NotNull(jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier));
        Assert.NotNull(jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email));
        Assert.NotNull(jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role));
    }

    [Fact]
    public async Task Jwt_Email_Claim_Matches_Registered_Email()
    {
        var token = await GetToken();
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var emailClaim = jwt.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        Assert.Equal("jwt@example.com", emailClaim);
    }

    [Fact]
    public async Task Jwt_Role_Claim_Is_USER()
    {
        var token = await GetToken();
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var roleClaim = jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value;
        Assert.Equal("USER", roleClaim);
    }

    [Fact]
    public async Task Jwt_Signature_Is_Valid_With_Known_Secret()
    {
        var token = await GetToken();

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Helpers.JwtSecret));
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = key,
        };

        var handler = new JwtSecurityTokenHandler();
        // Ne doit pas lever d'exception si la signature est correcte
        var principal = handler.ValidateToken(token, validationParams, out var validatedToken);

        Assert.NotNull(principal);
        Assert.IsType<JwtSecurityToken>(validatedToken);
    }

    [Fact]
    public async Task Jwt_Signature_Fails_With_Wrong_Secret()
    {
        var token = await GetToken();

        var wrongKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("MauvaisSecretQuiNeFonctionnePas!X"));
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            IssuerSigningKey = wrongKey,
        };

        var handler = new JwtSecurityTokenHandler();
        Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(
            () => handler.ValidateToken(token, validationParams, out _));
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// TEST 4 — Token de réinitialisation et expiration
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Vérifie la génération du token de mot de passe oublié et la vérification
/// de son expiration côté service.
///
/// Stratégie : on utilise EF Core InMemory pour simuler la table
/// password_reset_tokens sans base de données réelle.
/// </summary>
public class PasswordResetTokenTests
{
    // ── Test 4a ─────────────────────────────────────────────────────────────────
    /// <summary>
    /// ForgotPasswordAsync doit créer un token valide en base avec
    /// une expiration dans ~1 heure.
    /// </summary>
    [Fact]
    public async Task ForgotPassword_SavesToken_WithCorrectExpiration()
    {
        var (service, repoMock, context) = Helpers.Build();

        var existingUser = new UserModel { Id = 10, Email = "user@example.com", Password = "hash:sel", RoleId = 2 };
        repoMock
            .Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        var before = DateTime.UtcNow;
        await service.ForgotPasswordAsync(new ForgotPasswordDTO { Email = "user@example.com" });
        var after = DateTime.UtcNow;

        var saved = context.PasswordResetTokens.Single();

        // Token non vide et de longueur attendue (32 octets → 64 chars hex)
        Assert.NotEmpty(saved.Token);
        Assert.Equal(64, saved.Token.Length);

        // ExpiresAt est bien dans ~1 heure (tolérance de 5 secondes)
        Assert.InRange(saved.ExpiresAt, before.AddHours(1).AddSeconds(-5), after.AddHours(1).AddSeconds(5));

        // Token non utilisé à la création
        Assert.False(saved.Used);
    }

    // ── Test 4b ─────────────────────────────────────────────────────────────────
    /// <summary>
    /// ForgotPasswordAsync ne doit rien faire (pas d'exception, pas de token)
    /// lorsque l'email est inconnu — pour ne pas révéler l'existence d'un compte.
    /// </summary>
    [Fact]
    public async Task ForgotPassword_DoesNothing_WhenEmailUnknown()
    {
        var (service, repoMock, context) = Helpers.Build();

        repoMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);

        // Ne doit pas lever d'exception
        await service.ForgotPasswordAsync(new ForgotPasswordDTO { Email = "inconnu@example.com" });

        // Aucun token créé
        Assert.Empty(context.PasswordResetTokens);
    }

    // ── Test 4c ─────────────────────────────────────────────────────────────────
    /// <summary>
    /// ResetPasswordAsync doit lever une InvalidOperationException
    /// si le token est expiré (ExpiresAt dans le passé).
    /// </summary>
    [Fact]
    public async Task ResetPassword_Throws_WhenTokenExpired()
    {
        var (service, repoMock, context) = Helpers.Build();

        // On insère directement un token expiré en base
        context.PasswordResetTokens.Add(new PasswordResetTokenModel
        {
            UserId = 1,
            Token = "tokenexpiré_" + Convert.ToHexString(RandomNumberGenerator.GetBytes(8)),
            ExpiresAt = DateTime.UtcNow.AddHours(-2), // déjà expiré
            Used = false,
        });
        await context.SaveChangesAsync();

        var expiredToken = context.PasswordResetTokens.First().Token;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ResetPasswordAsync(new ResetPasswordDTO { Token = expiredToken, NewPassword = "NouveauPass!1" }));

        Assert.Contains("expiré", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ── Test 4d ─────────────────────────────────────────────────────────────────
    /// <summary>
    /// ResetPasswordAsync doit lever une InvalidOperationException
    /// si le token a déjà été utilisé (Used = true).
    /// </summary>
    [Fact]
    public async Task ResetPassword_Throws_WhenTokenAlreadyUsed()
    {
        var (service, repoMock, context) = Helpers.Build();

        context.PasswordResetTokens.Add(new PasswordResetTokenModel
        {
            UserId = 1,
            Token = "tokendejautilise_" + Convert.ToHexString(RandomNumberGenerator.GetBytes(8)),
            ExpiresAt = DateTime.UtcNow.AddHours(1), // pas encore expiré
            Used = true, // mais déjà consommé
        });
        await context.SaveChangesAsync();

        var usedToken = context.PasswordResetTokens.First().Token;

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ResetPasswordAsync(new ResetPasswordDTO { Token = usedToken, NewPassword = "NouveauPass!1" }));
    }

    // ── Test 4e ─────────────────────────────────────────────────────────────────
    /// <summary>
    /// ResetPasswordAsync avec un token valide doit mettre à jour le mot de passe
    /// et marquer le token comme utilisé (Used = true).
    /// </summary>
    [Fact]
    public async Task ResetPassword_MarksTokenAsUsed_AndUpdatesPassword()
    {
        var (service, repoMock, context) = Helpers.Build();

        var validToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        context.PasswordResetTokens.Add(new PasswordResetTokenModel
        {
            UserId = 99,
            Token = validToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Used = false,
        });
        await context.SaveChangesAsync();

        var user = new UserModel { Id = 99, Email = "reset@example.com", Password = "ancienHash:ancienSel", RoleId = 2 };
        repoMock
            .Setup(r => r.FindAsync<int>(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        repoMock
            .Setup(r => r.UpdateAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await service.ResetPasswordAsync(new ResetPasswordDTO { Token = validToken, NewPassword = "NouveauPass!1" });

        // Le token est marqué Used = true
        var savedToken = context.PasswordResetTokens.Single();
        Assert.True(savedToken.Used);

        // Le nouveau mot de passe est haché (≠ ancien, ≠ en clair)
        Assert.NotEqual("ancienHash:ancienSel", user.Password);
        Assert.NotEqual("NouveauPass!1", user.Password);
        Assert.Contains(":", user.Password); // format hash:sel
    }
}
