using Microsoft.Extensions.Configuration;
using Platform.Lib.Core.Services.Security;
using System.Security.Cryptography;
using System.Text;

namespace Platform.Lib.Infrastructure.Services.Security
{
    public class EncryptService : IEncryptService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptService(IConfiguration configuration)
        {
            var keyString = configuration["Security:SecretKey"]
                ?? throw new InvalidOperationException(
                    "Security:SecretKey is not configured.");

            if (!Guid.TryParse(keyString, out var guid))
                throw new InvalidOperationException(
                    "Security:SecretKey must be a valid GUID.");

            _key = SHA256.HashData(guid.ToByteArray());
            _iv = new byte[16];
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using var aes = Aes.Create();

            aes.Key = _key;
            aes.IV = _iv;

            using var encryptor = aes.CreateEncryptor();

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = encryptor.TransformFinalBlock(
                plainBytes,
                0,
                plainBytes.Length);

            return Convert.ToBase64String(cipherBytes);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            using var aes = Aes.Create();

            aes.Key = _key;
            aes.IV = _iv;

            using var decryptor = aes.CreateDecryptor();

            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            byte[] plainBytes = decryptor.TransformFinalBlock(
                cipherBytes,
                0,
                cipherBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}


