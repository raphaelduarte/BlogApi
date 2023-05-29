namespace BlogApi
{
    public static class Configuration
    {
        // TOKEN - JWT - JSON Web Token
        public static string JwtKey = "Zj5sozHNSIapCiGm7YqJbQ==";
        public static string ApiKeyName = "api_key";
        public static string ApiKey = $"api_{JwtKey}";
        public static SmtpConfiguration Smtp = new();


        public class SmtpConfiguration
        {
            public string Host { get; set; }
            public int Port { get; set; } = 25;
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
}

