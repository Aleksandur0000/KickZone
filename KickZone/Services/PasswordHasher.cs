using System.Security.Cryptography;

namespace KickZone.Services
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100_000;

        public static string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
            return Convert.ToBase64String(salt.Concat(hash).ToArray());
        }

        public static bool Verify(string password, string storedHash)
        {
            try
            {
                var data = Convert.FromBase64String(storedHash);
                var salt = data[..SaltSize];
                var expected = data[SaltSize..];
                var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch
            {
                return false;
            }
        }
    }
}
