namespace Platform.Lib.Core.Services
{
    public interface ILocalizationService
    {
        string Get(string key);
        string Get(string key, params object[] arguments);
    }
}
