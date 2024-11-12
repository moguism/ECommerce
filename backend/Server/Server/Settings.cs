namespace Server
{
    public class Settings
    {
        public const string SECTION_NAME = "Settings";
        public string DatabaseConnection { get; init; }
        public string ClientBaseUrl { get; init; } = "https://farminhouse.com"; // Esto no sirve, debe dirigir de vuelta al front
        public string JwtKey { get; init; }
        public string StripeSecret { get; init; }
    }
}
