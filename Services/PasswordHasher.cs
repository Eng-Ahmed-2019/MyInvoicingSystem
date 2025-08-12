using System.Security.Cryptography;

namespace InvoicingSystem.Services
{
    public class PasswordHasher
    {
        private const int Iterations = 10000;
        private const int SaltSize = 16;
        private const int KeySize = 32;

        public static string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(KeySize);

            var result = new byte[SaltSize + KeySize];
            Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
            Buffer.BlockCopy(key, 0, result, SaltSize, KeySize);

            return Convert.ToBase64String(result);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            var hashBytes = Convert.FromBase64String(storedHash);

            var salt = new byte[SaltSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);

            var storedkey = new byte[KeySize];
            Buffer.BlockCopy(hashBytes, SaltSize, storedkey, 0, KeySize);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(KeySize);

            return CryptographicOperations.FixedTimeEquals(storedkey, key);
        }
    }
}