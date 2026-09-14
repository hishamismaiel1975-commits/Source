namespace Platform.Lib.Core.Services.Security
{
    public interface IEncryptService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
