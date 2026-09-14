using Platform.Lib.Core.Services.Security;
using System.Security.Cryptography;

namespace Platform.Lib.Infrastructure.Services.Security
{
    public class HashService : IHashService
    {
        private const int SaltSize = 16;       // 128-bit
        private const int KeySize = 32;        // 256-bit
        private const int Iterations = 600_000;

        public string Hash(string password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(password);

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            // Store: version.iterations.salt.hash
            return $"PBKDF2-SHA256.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool HashVerify(string value, string valueHash)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            ArgumentException.ThrowIfNullOrWhiteSpace(valueHash);

            string[] parts = valueHash.Split('.');

            if (parts.Length != 4)
                return false;

            if (parts[0] != "PBKDF2-SHA256")
                return false;

            if (!int.TryParse(parts[1], out int iterations))
                return false;

            try
            {
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] storedHash = Convert.FromBase64String(parts[3]);

                byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
                    value,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    storedHash.Length);

                return CryptographicOperations.FixedTimeEquals(
                    computedHash,
                    storedHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}

