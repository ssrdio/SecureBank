using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Ctf
{
    public static class CtfConstants
    {
        public static readonly string[] XXS_KEYVORDS = new string[]
        {
            "<script>",
            "<iframe ",
            " src="
        };
    }
}
