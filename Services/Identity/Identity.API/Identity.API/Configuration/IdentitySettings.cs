namespace Identity.API.Configuration
{
    public class IdentitySettings
    {
        public string? ConnectionString { get; set; }
        public string? SecretKey { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int? AccessTokenLifetimeInMinutes { get; set; }
        public int? RefreshTokenLifetimeInMinutes { get; set; }

    }
}

