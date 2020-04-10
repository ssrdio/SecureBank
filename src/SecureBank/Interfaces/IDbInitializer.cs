using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface IDbInitializer
    {
        void Initialize(IApplicationBuilder app, string admin, string adminPassword, string userPassword);

        void Create(IApplicationBuilder app);
    }
}
