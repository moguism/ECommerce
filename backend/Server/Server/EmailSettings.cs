namespace Server
{
    public class EmailSettings
    {
        public const string SECTION_NAME = "EmailSettings";
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string EmailFrom { get; set; }
        public string PasswordEmailFrom { get; set; }
    }
}
