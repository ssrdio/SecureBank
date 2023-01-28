using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Ctf.Models
{
    public class CtfOptions
    {
        public bool IsCtfEnabled { get; set; }
        public List<CtfChallengeModel> CtfChallenges { get; set; }

        public CtfChallengeOptions CtfChallengeOptions { get; set; }

        public CtfOptions()
        {
        }

        public CtfOptions(List<CtfChallengeModel> ctfChallenges, CtfChallengeOptions ctfChallengeOptions)
        {
            CtfChallenges = ctfChallenges;
            CtfChallengeOptions = ctfChallengeOptions;
        }
    }
}
