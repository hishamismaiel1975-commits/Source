using Microsoft.Extensions.Configuration;
using Platform.Lib.Core.Services.Security;
using System.Security.Cryptography;
using System.Text;

namespace Platform.Lib.Infrastructure.Services.Security
{
    public class HashService : IHashService
    {
        private readonly byte[] _key;

        public HashService(IConfiguration configuration)
        {
            var key = configuration["Security:SecretKey"]
                ?? throw new InvalidOperationException(
                    "Security:SecretKey is not configured.");

            _key = Encoding.UTF8.GetBytes(key);
        }

        public string Hash(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            using var hmac = new HMACSHA256(_key);

            var hash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(value));

            return Convert.ToBase64String(hash);
        }

        public bool Verify(string value, string valueHash)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            ArgumentException.ThrowIfNullOrWhiteSpace(valueHash);

            var computedHash = Convert.FromBase64String(Hash(value));
            var storedHash = Convert.FromBase64String(valueHash);

            return CryptographicOperations.FixedTimeEquals(
                computedHash,
                storedHash);
        }
    }
}


