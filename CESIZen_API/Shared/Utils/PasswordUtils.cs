// Utilitaires de hachage et vérification de mot de passe.
// Utilise PBKDF2-SHA512 avec 350 000 itérations et un sel aléatoire de 64 octets.
// Le mot de passe stocké en base suit le format : "hash_hex:sel_hex"
// (assemblé par UserService, pas par cette classe).
// CryptographicOperations.FixedTimeEquals protège contre les attaques par timing.

using System.Security.Cryptography;
using System.Text;

namespace CESIZen_API.Shared.Utils
{
    public class PasswordUtils
    {
        // Taille du sel et de la clé dérivée en octets (64 octets = 512 bits)
        private const int keySize = 64;
        // Nombre d'itérations PBKDF2 (NIST recommande ≥ 310 000 pour SHA-512 en 2023)
        private const int iterations = 350000;

        /// <summary>
        /// Hache un mot de passe avec PBKDF2-SHA512 et un sel aléatoire.
        /// </summary>
        /// <param name="password">Mot de passe en clair à hacher.</param>
        /// <param name="salt">Sel aléatoire généré (64 octets), retourné via paramètre out.</param>
        /// <returns>Hash hexadécimal de 128 caractères (64 octets).</returns>
        public static string HashPassword(string password, out byte[] salt)
        {
            HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

            // Génération d'un sel cryptographiquement aléatoire à chaque appel
            salt = RandomNumberGenerator.GetBytes(keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);

            return Convert.ToHexString(hash);
        }

        /// <summary>
        /// Vérifie qu'un mot de passe en clair correspond au hash stocké.
        /// Utilise une comparaison en temps constant pour prévenir les attaques par timing.
        /// </summary>
        /// <param name="password">Mot de passe en clair à vérifier.</param>
        /// <param name="hash">Hash hexadécimal stocké (extrait du format "hash:sel").</param>
        /// <param name="salt">Sel utilisé lors du hachage original.</param>
        /// <returns>true si le mot de passe correspond, false sinon.</returns>
        public static bool VerifyPassword(string password, string hash, byte[] salt)
        {
            HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
            // FixedTimeEquals : comparaison en temps constant (résistant aux timing attacks)
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }
    }
}
