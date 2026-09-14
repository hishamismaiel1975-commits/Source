using Microsoft.Extensions.Configuration;
using Platform.Lib.Core.Services.Security;
using System.Security.Cryptography;
using System.Text;

namespace Platform.Lib.Infrastructure.Services.Security
{
    public class EncryptService : IEncryptService
    {
        private readonly byte[] _key;

        public EncryptService(IConfiguration configuration)
        {
            var keyString = configuration["Security:SecretKey"]
                ?? throw new InvalidOperationException(
                    "Security:SecretKey is not configured.");

            if (!Guid.TryParse(keyString, out var key))
                throw new InvalidOperationException(
                    "Security:SecretKey must be a valid GUID.");

            _key = SHA256.HashData(key.ToByteArray());
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            byte[] nonce = RandomNumberGenerator.GetBytes(12);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[16];

            using var aes = new AesGcm(_key, 16);

            aes.Encrypt(
                nonce,
                plainBytes,
                cipherBytes,
                tag);

            byte[] result = new byte[
                nonce.Length +
                tag.Length +
                cipherBytes.Length];

            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
            Buffer.BlockCopy(
                cipherBytes,
                0,
                result,
                nonce.Length + tag.Length,
                cipherBytes.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            byte[] data = Convert.FromBase64String(cipherText);

            const int nonceSize = 12;
            const int tagSize = 16;

            if (data.Length < nonceSize + tagSize)
                throw new CryptographicException(
                    "Invalid encrypted data.");

            byte[] nonce = data[..nonceSize];
            byte[] tag = data[nonceSize..(nonceSize + tagSize)];
            byte[] cipherBytes = data[(nonceSize + tagSize)..];

            byte[] plainBytes = new byte[cipherBytes.Length];

            using var aes = new AesGcm(_key, 16);

            aes.Decrypt(
                nonce,
                cipherBytes,
                tag,
                plainBytes);

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}


