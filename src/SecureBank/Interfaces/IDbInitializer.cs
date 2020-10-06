using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface IDbInitializer
    {
        void Initialize(string admin, string adminPassword);
        void Seed(string userPassword);

        void Create(IApplicationBuilder app);
    }
}
