using SecureBank.Ctf.Models;

namespace SecureBank.Models
{
    public class CtfConfig
    {
        public string Seed { get; set; }
        public bool Enabled { get; set; }
        public bool GenerateCtfdExport { get; set; }

        public string FlagFormat { get; set; }
        public bool UseRealChallengeName { get; set; }

        public string FtpFlag { get; set; }

        public CtfChallengeOptions Challenges { get; set; }
    }
}
