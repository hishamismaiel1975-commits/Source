namespace Platform.Lib.Core.Services.Security
{
    public interface IHashService
    {
        string Hash(string value);
        bool Verify(string value, string valueHash);
    }
}
