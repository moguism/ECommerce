namespace Server
{
    public class Settings
    {
        public const string SECTION_NAME = "Settings";
        public string DatabaseConnection { get; init; }
        public string ClientBaseUrl { get; init; } = "http://localhost:4200"; // TODO: Cambiar esto
        public string JwtKey { get; init; }
        public string StripeSecret { get; init; }
    }
}
