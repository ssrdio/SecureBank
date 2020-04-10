using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Ctf.CTFd.Models
{
    public class CTFdBaseModel<TData>
    {
        public int Count { get; set; }
        public List<TData> Results { get; set; }
        public object Meta { get; set; }

        public CTFdBaseModel(List<TData> results)
        {
            Results = results;
            Count = results.Count;
        }

        public CTFdBaseModel(TData result)
        {
            Results = new List<TData> { result };
            Count = Results.Count;
        }
    }
}
