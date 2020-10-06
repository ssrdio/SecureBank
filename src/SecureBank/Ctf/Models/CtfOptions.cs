using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Ctf.Models
{
    public class CtfOptions
    {
        public List<CtfChallangeModel> CtfChallanges { get; set; }

        public CtfChallengeOptions CtfChallengeOptions { get; set; }

        public CtfOptions()
        {
        }

        public CtfOptions(List<CtfChallangeModel> ctfChallanges, CtfChallengeOptions ctfChallengeOptions)
        {
            CtfChallanges = ctfChallanges;
            CtfChallengeOptions = ctfChallengeOptions;
        }
    }
}
