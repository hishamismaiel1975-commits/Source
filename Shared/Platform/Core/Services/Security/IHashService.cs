namespace Platform.Lib.Core.Services.Security
{
    public interface IHashService
    {
        string Hash(string value);
        bool HashVerify(string value, string valueHash);
    }
}
