namespace StoreAPI.Models
{
    public class CtfSettings
    {
        public bool Enabled { get; set; }
        public string Seed { get; set; }
        public string FlagFormat { get; set; }
        public string GeneratedFlag { get; set; }
        public bool SsrfChallenge { get; set; }
    }
}

