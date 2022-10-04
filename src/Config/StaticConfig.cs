namespace shopping_bag.Config
{
    public static class StaticConfig
    {
        public static int RefreshTokenValidityDays { get; private set; }
        public static int TokenValidityInMinutes { get; private set; }
        public static string Token { get; private set; }
        public static string Issuer { get; private set; }
        public static string Audience { get; private set; }
        public static string[] AllowedOrigins { get; private set; }
        public static string[] AllowedEmailDomain { get; private set; }
        public static void Setup(IConfiguration config)
        {
            RefreshTokenValidityDays = config.GetValue<int>("Jwt:RefreshTokenValidityDays");
            TokenValidityInMinutes = config.GetValue<int>("Jwt:TokenValidityMinutes");
            Token = config.GetValue<string>("Jwt:Token");
            Issuer = config.GetValue<string>("Jwt:Issuer");
            Audience = config.GetValue<string>("Jwt:Audience");
            AllowedOrigins = config.GetSection("AllowedOrigins").Get<string[]>();
            AllowedEmailDomain = config.GetSection("AllowedEmailDomain").Get<string[]>();
        }
    }
}
