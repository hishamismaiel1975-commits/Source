namespace Platform.Core.Localization
{
    public interface ILocalizationService
    {
        string Get(string key);
        string Get(string key, params object[] arguments);
    }
}
